using BssRms.Application.DTOs.Table;

namespace BssRms.Application.Interfaces;

public interface ITableService
{
    Task<TableDto> CreateAsync(CreateTableDto dto);
    Task<TableDetailDto?> GetByIdAsync(int id);
    Task<List<TableSimpleDto>> GetAllAsync();
    Task<TableDatatableSimpleDto> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<TableDto> UpdateAsync(int id, UpdateTableDto dto);
    Task<bool> DeleteAsync(int id);
}
