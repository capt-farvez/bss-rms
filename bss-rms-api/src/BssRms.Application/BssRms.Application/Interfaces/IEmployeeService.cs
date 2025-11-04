using BssRms.Application.DTOs.Employee;

namespace BssRms.Application.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<List<EmployeeListDto>> GetAllAsync();
    Task<EmployeeDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<List<NonAssignedEmployeeDto>> GetNonAssignedEmployeesAsync(int tableId);
}
