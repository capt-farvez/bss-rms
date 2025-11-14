using BssRms.Application.DTOs.EmployeeTable;

namespace BssRms.Application.Interfaces;

public interface IEmployeeTableService
{
    Task<EmployeeTableDto> CreateAsync(CreateEmployeeTableDto dto);
    Task<string> CreateRangeAsync(List<CreateEmployeeTableDto> dtos);
    Task<EmployeeTableDto?> GetByIdAsync(int id);
    Task<List<EmployeeTableListDto>> GetAllAsync();
    Task<EmployeeTableDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<EmployeeTableDto> UpdateAsync(int id, UpdateEmployeeTableDto dto);
    Task<bool> DeleteAsync(int id);
}
