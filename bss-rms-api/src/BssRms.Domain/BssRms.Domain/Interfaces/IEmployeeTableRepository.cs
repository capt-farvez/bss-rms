using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface IEmployeeTableRepository
{
    Task<EmployeeTable> CreateAsync(EmployeeTable employeeTable);
    Task<List<EmployeeTable>> CreateRangeAsync(List<EmployeeTable> employeeTables);
    Task<EmployeeTable?> GetByIdAsync(int id);
    Task<List<EmployeeTable>> GetAllAsync();
    Task<(List<EmployeeTable> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<EmployeeTable> UpdateAsync(EmployeeTable employeeTable);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(Guid employeeId, int tableId);
    Task<bool> ExistsAsync(Guid employeeId, int tableId, int excludeId);
}
