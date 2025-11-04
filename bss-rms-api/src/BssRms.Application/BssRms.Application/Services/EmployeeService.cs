using BssRms.Application.DTOs.Employee;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;

namespace BssRms.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;

    public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository)
    {
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        try
        {
            var user = new User
            {
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                FatherName = dto.FatherName,
                MotherName = dto.MotherName,
                SpouseName = dto.SpouseName,
                Dob = dto.Dob,
                Nid = dto.Nid,
                GenderId = dto.GenderId,
                Image = dto.Image,
                ImageBase64 = dto.Base64
            };

            var createdUser = await _userRepository.CreateAsync(user);

            var employee = new Employee
            {
                UserId = createdUser.Uid,
                Designation = dto.Designation,
                JoinDate = dto.JoinDate
            };

            var createdEmployee = await _employeeRepository.CreateAsync(employee);
            var result = await _employeeRepository.GetByIdAsync(createdEmployee.EmployeeId);

            return MapToDto(result!);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating employee: {ex.Message}", ex);
        }
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee == null ? null : MapToDto(employee);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employee: {ex.Message}", ex);
        }
    }

    public async Task<List<EmployeeListDto>> GetAllAsync()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(MapToListDto).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employees: {ex.Message}", ex);
        }
    }

    public async Task<EmployeeDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        try
        {
            var (data, totalRecords) = await _employeeRepository.GetDatatableAsync(page, perPage, search, sort);
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            return new EmployeeDatatableDto
            {
                Data = data.Select(MapToDto).ToList(),
                CurrentPage = page,
                PerPage = perPage,
                Total = totalRecords,
                LastPage = lastPage
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employee datatable: {ex.Message}", ex);
        }
    }

    public async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found");

            if (!string.IsNullOrWhiteSpace(dto.Designation))
                employee.Designation = dto.Designation;

            if (dto.JoinDate.HasValue)
                employee.JoinDate = dto.JoinDate.Value;

            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);
            var result = await _employeeRepository.GetByIdAsync(updatedEmployee.EmployeeId);

            return MapToDto(result!);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error updating employee: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var exists = await _employeeRepository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Employee with ID {id} not found");

            return await _employeeRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error deleting employee: {ex.Message}", ex);
        }
    }

    public async Task<List<NonAssignedEmployeeDto>> GetNonAssignedEmployeesAsync(int tableId)
    {
        try
        {
            var employees = await _employeeRepository.GetNonAssignedEmployeesAsync(tableId);
            return employees.Select(e => new NonAssignedEmployeeDto
            {
                EmployeeId = e.EmployeeId,
                Name = e.User != null ? $"{e.User.FirstName} {e.User.LastName}".Trim() : string.Empty
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving non-assigned employees: {ex.Message}", ex);
        }
    }

    private EmployeeDto MapToDto(Employee employee)
    {
        var user = employee.User;
        var fullName = user != null
            ? $"{user.FirstName} {user.LastName}".Trim()
            : string.Empty;

        return new EmployeeDto
        {
            Id = employee.EmployeeId,
            Designation = employee.Designation,
            JoinDate = employee.JoinDate,
            AmountSold = employee.AmountSold,
            User = new UserInfoDto
            {
                Id = user?.Uid ?? Guid.Empty,
                UserName = user?.UserName,
                Email = user?.Email ?? string.Empty,
                FullName = fullName,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                FirstName = user?.FirstName ?? string.Empty,
                LastName = user?.LastName ?? string.Empty,
                Image = user?.Image
            }
        };
    }

    private EmployeeListDto MapToListDto(Employee employee)
    {
        var user = employee.User;
        var name = user != null
            ? $"{user.FirstName} {user.LastName}".Trim()
            : string.Empty;

        return new EmployeeListDto
        {
            EmployeeId = employee.EmployeeId,
            Name = name
        };
    }
}
