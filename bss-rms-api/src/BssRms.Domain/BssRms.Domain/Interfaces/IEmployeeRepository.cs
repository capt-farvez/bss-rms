using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee?> GetByIdAsync(Guid id);
    Task<List<Employee>> GetAllAsync();
    Task<(List<Employee> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<Employee> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<List<Employee>> GetNonAssignedEmployeesAsync(int tableId);
}
