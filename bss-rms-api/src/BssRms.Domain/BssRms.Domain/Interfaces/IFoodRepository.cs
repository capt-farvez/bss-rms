using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface IFoodRepository
{
    Task<Food> CreateAsync(Food food);
    Task<Food?> GetByIdAsync(int id);
    Task<List<Food>> GetAllAsync();
    Task<(List<Food> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<Food> UpdateAsync(Food food);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
}
