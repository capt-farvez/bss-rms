using BssRms.Application.DTOs.Employee;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            // Store just the filename, remove any path prefix
            var imageName = dto.Image;
            if (!string.IsNullOrEmpty(imageName))
            {
                imageName = imageName.Replace("/images/user/", "").Replace("\\images\\user\\", "");
            }

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
                Image = imageName ?? string.Empty,
                ImageBase64 = dto.Base64 ?? string.Empty
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
            var query = _employeeRepository.QueryEmployees();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e =>
                    e.User.FirstName.Contains(search) ||
                    e.User.LastName.Contains(search) ||
                    e.User.Email.Contains(search) ||
                    e.User.PhoneNumber.Contains(search) ||
                    e.Designation.Contains(search));
            }

            // Get count before pagination
            var totalRecords = await query.CountAsync();
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortParts = sort.Split(' ');
                var sortField = sortParts[0];
                var sortDirection = sortParts.Length > 1 ? sortParts[1].ToLower() : "asc";

                query = sortField.ToLower() switch
                {
                    "firstname" => sortDirection == "desc"
                        ? query.OrderByDescending(e => e.User.FirstName)
                        : query.OrderBy(e => e.User.FirstName),
                    "lastname" => sortDirection == "desc"
                        ? query.OrderByDescending(e => e.User.LastName)
                        : query.OrderBy(e => e.User.LastName),
                    "email" => sortDirection == "desc"
                        ? query.OrderByDescending(e => e.User.Email)
                        : query.OrderBy(e => e.User.Email),
                    "designation" => sortDirection == "desc"
                        ? query.OrderByDescending(e => e.Designation)
                        : query.OrderBy(e => e.Designation),
                    "joindate" => sortDirection == "desc"
                        ? query.OrderByDescending(e => e.JoinDate)
                        : query.OrderBy(e => e.JoinDate),
                    _ => query.OrderBy(e => e.CreatedAt)
                };
            }
            else
            {
                query = query.OrderBy(e => e.CreatedAt);
            }

            // Paginate and project directly into DTOs — single SQL query
            var employeeDtos = await query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .Select(e => new EmployeeDto
                {
                    Id = e.EmployeeId,
                    Designation = e.Designation,
                    JoinDate = e.JoinDate,
                    AmountSold = e.AmountSold,
                    User = new UserInfoDto
                    {
                        Id = e.User != null ? e.User.Uid : Guid.Empty,
                        UserName = e.User != null ? e.User.UserName : null,
                        Email = e.User != null ? e.User.Email : "",
                        FullName = e.User != null
                            ? (e.User.FirstName ?? "") + " " + (e.User.LastName ?? "")
                            : "",
                        PhoneNumber = e.User != null ? e.User.PhoneNumber : "",
                        FirstName = e.User != null ? e.User.FirstName ?? "" : "",
                        MiddleName = e.User != null ? e.User.MiddleName : null,
                        LastName = e.User != null ? e.User.LastName ?? "" : "",
                        FatherName = e.User != null ? e.User.FatherName : null,
                        MotherName = e.User != null ? e.User.MotherName : null,
                        SpouseName = e.User != null ? e.User.SpouseName : null,
                        Dob = e.User != null ? e.User.Dob : null,
                        Nid = e.User != null ? e.User.Nid : null,
                        GenderId = e.User != null ? e.User.GenderId : 0,
                        Image = e.User != null ? e.User.Image : null
                    }
                })
                .ToListAsync();

            return new EmployeeDatatableDto
            {
                Data = employeeDtos,
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

            // Update Employee fields
            if (!string.IsNullOrWhiteSpace(dto.Designation))
                employee.Designation = dto.Designation;

            if (dto.JoinDate.HasValue)
                employee.JoinDate = dto.JoinDate.Value;

            // Update User fields
            var user = employee.User;
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.Email))
                    user.Email = dto.Email;

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                    user.PhoneNumber = dto.PhoneNumber;

                if (!string.IsNullOrWhiteSpace(dto.FirstName))
                    user.FirstName = dto.FirstName;

                if (!string.IsNullOrWhiteSpace(dto.MiddleName))
                    user.MiddleName = dto.MiddleName;

                if (!string.IsNullOrWhiteSpace(dto.LastName))
                    user.LastName = dto.LastName;

                if (!string.IsNullOrWhiteSpace(dto.FatherName))
                    user.FatherName = dto.FatherName;

                if (!string.IsNullOrWhiteSpace(dto.MotherName))
                    user.MotherName = dto.MotherName;

                if (!string.IsNullOrWhiteSpace(dto.SpouseName))
                    user.SpouseName = dto.SpouseName;

                if (dto.Dob.HasValue)
                    user.Dob = dto.Dob.Value;

                if (!string.IsNullOrWhiteSpace(dto.Nid))
                    user.Nid = dto.Nid;

                if (dto.GenderId.HasValue)
                    user.GenderId = dto.GenderId.Value;

                // Update image if provided
                if (!string.IsNullOrWhiteSpace(dto.Image) || !string.IsNullOrWhiteSpace(dto.Base64))
                {
                    // Store just the filename, remove any path prefix
                    var imageName = dto.Image;
                    if (!string.IsNullOrEmpty(imageName))
                    {
                        imageName = imageName.Replace("/images/user/", "").Replace("\\images\\user\\", "");
                    }

                    user.Image = imageName ?? string.Empty;
                    user.ImageBase64 = dto.Base64 ?? string.Empty;
                }

                await _userRepository.UpdateAsync(user);
            }

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
                MiddleName = user?.MiddleName,
                LastName = user?.LastName ?? string.Empty,
                FatherName = user?.FatherName,
                MotherName = user?.MotherName,
                SpouseName = user?.SpouseName,
                Dob = user?.Dob,
                Nid = user?.Nid,
                GenderId = user?.GenderId ?? 0,
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
