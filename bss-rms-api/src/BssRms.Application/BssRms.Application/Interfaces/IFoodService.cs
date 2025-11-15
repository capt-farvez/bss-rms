using BssRms.Application.DTOs.Food;

namespace BssRms.Application.Interfaces;

public interface IFoodService
{
    Task<FoodDto> CreateAsync(CreateFoodDto dto);
    Task<FoodDetailDto?> GetByIdAsync(int id);
    Task<List<FoodSimpleDto>> GetAllAsync();
    Task<FoodDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<FoodDetailDto> UpdateAsync(int id, UpdateFoodDto dto);
    Task<bool> DeleteAsync(int id);
}
