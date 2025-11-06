using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface ITableRepository
{
    Task<Table> CreateAsync(Table table);
    Task<Table?> GetByIdAsync(int id);
    Task<List<Table>> GetAllAsync();
    Task<(List<Table> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<Table> UpdateAsync(Table table);
    Task<bool> DeleteAsync(int id);
}
