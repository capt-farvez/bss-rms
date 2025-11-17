using BssRms.Application.DTOs.Food;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;

namespace BssRms.Application.Services;

public class FoodService : IFoodService
{
    private readonly IFoodRepository _foodRepository;

    public FoodService(IFoodRepository foodRepository)
    {
        _foodRepository = foodRepository;
    }

    public async Task<FoodDto> CreateAsync(CreateFoodDto dto)
    {
        try
        {
            if (await _foodRepository.ExistsByNameAsync(dto.Name))
            {
                throw new InvalidOperationException($"Food with name '{dto.Name}' already exists.");
            }

            // Parse discount type from string to int
            int discountType = dto.DiscountType.ToLower() switch
            {
                "percentage" => 1,
                "flat" => 2,
                "fixed" => 2,
                _ => 0 // None
            };

            // Store just the filename, remove any path prefix
            var imageName = dto.Image;
            if (!string.IsNullOrEmpty(imageName))
            {
                imageName = imageName.Replace("/images/food/", "").Replace("\\images\\food\\", "");
            }

            var food = new Food
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DiscountType = discountType,
                Discount = dto.Discount,
                Image = imageName ?? string.Empty,
                ImageBase64 = dto.Base64 ?? string.Empty
            };

            var createdFood = await _foodRepository.CreateAsync(food);
            return MapToDto(createdFood);
        }
        catch (Exception ex)
        {
            throw ex is InvalidOperationException ? ex : new Exception($"Error creating food: {ex.Message}", ex);
        }
    }

    public async Task<FoodDetailDto?> GetByIdAsync(int id)
    {
        try
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null){
                return null;
            }

            decimal discountPrice = food.Price;
            string discountTypeStr = "None";

            switch (food.DiscountType)
            {
                case 1: // Percentage
                    discountTypeStr = "Percentage";
                    discountPrice = food.Price - (food.Price * food.Discount / 100);
                    break;
                case 2: // Flat/Fixed
                    discountTypeStr = "Flat";
                    discountPrice = food.Price - food.Discount;
                    break;
                default: // None
                    discountTypeStr = "None";
                    break;
            }

            return new FoodDetailDto
            {
                Id = food.FoodId,
                Name = food.Name,
                Description = food.Description,
                Price = food.Price,
                DiscountType = discountTypeStr,
                Discount = food.Discount,
                DiscountPrice = discountPrice,
                Image = food.Image
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving food: {ex.Message}", ex);
        }
    }

    public async Task<List<FoodSimpleDto>> GetAllAsync()
    {
        try
        {
            var foods = await _foodRepository.GetAllAsync();
            return foods.Select(f => new FoodSimpleDto
            {
                FoodId = f.FoodId,
                Name = f.Name
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving foods: {ex.Message}", ex);
        }
    }

    public async Task<FoodDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        try
        {
            var (data, totalRecords) = await _foodRepository.GetDatatableAsync(page, perPage, search, sort);
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            var foodDtos = data.Select(food =>
            {
                // Calculate discount price based on discount type
                decimal discountPrice = food.Price;
                string discountTypeStr = "None";

                switch (food.DiscountType)
                {
                    case 1: // Percentage
                        discountTypeStr = "Percentage";
                        discountPrice = food.Price - (food.Price * food.Discount / 100);
                        break;
                    case 2: // Flat/Fixed
                        discountTypeStr = "Flat";
                        discountPrice = food.Price - food.Discount;
                        break;
                    default: // None
                        discountTypeStr = "None";
                        break;
                }

                return new FoodDatatableItemDto
                {
                    Id = food.FoodId,
                    Name = food.Name,
                    Description = food.Description,
                    Price = food.Price,
                    DiscountType = discountTypeStr,
                    Discount = food.Discount,
                    DiscountPrice = discountPrice,
                    Image = food.Image
                };
            }).ToList();

            return new FoodDatatableDto
            {
                Data = foodDtos,
                CurrentPage = page,
                PerPage = perPage,
                Total = totalRecords,
                LastPage = lastPage
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving food datatable: {ex.Message}", ex);
        }
    }

    public async Task<FoodDetailDto> UpdateAsync(int id, UpdateFoodDto dto)
    {
        try
        {
            var food = await _foodRepository.GetByIdAsync(id);
            if (food == null)
            {
                throw new KeyNotFoundException($"Food with ID {id} not found.");
            }

            if (await _foodRepository.ExistsByNameAsync(dto.Name, id))
            {
                throw new InvalidOperationException($"Food with name '{dto.Name}' already exists.");
            }

            // Parse discount type from string to int
            int discountType = dto.DiscountType.ToLower() switch
            {
                "percentage" => 1,
                "flat" => 2,
                "fixed" => 2,
                _ => 0 // None
            };

            food.Name = dto.Name;
            food.Description = dto.Description;
            food.Price = dto.Price;
            food.DiscountType = discountType;
            food.Discount = dto.Discount;

            // Only update image if provided
            if (!string.IsNullOrEmpty(dto.Image))
            {
                var imageName = dto.Image.Replace("/images/food/", "").Replace("\\images\\food\\", "");
                food.Image = imageName;
            }

            if (!string.IsNullOrEmpty(dto.Base64))
            {
                food.ImageBase64 = dto.Base64;
            }

            var updatedFood = await _foodRepository.UpdateAsync(food);

            // Calculate discount price based on discount type
            decimal discountPrice = updatedFood.Price;
            string discountTypeStr = "None";

            switch (updatedFood.DiscountType)
            {
                case 1: // Percentage
                    discountTypeStr = "Percentage";
                    discountPrice = updatedFood.Price - (updatedFood.Price * updatedFood.Discount / 100);
                    break;
                case 2: // Flat/Fixed
                    discountTypeStr = "Flat";
                    discountPrice = updatedFood.Price - updatedFood.Discount;
                    break;
                default: // None
                    discountTypeStr = "None";
                    break;
            }

            return new FoodDetailDto
            {
                Id = updatedFood.FoodId,
                Name = updatedFood.Name,
                Description = updatedFood.Description,
                Price = updatedFood.Price,
                DiscountType = discountTypeStr,
                Discount = updatedFood.Discount,
                DiscountPrice = discountPrice,
                Image = updatedFood.Image
            };
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException || ex is InvalidOperationException ? ex : new Exception($"Error updating food: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var exists = await _foodRepository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Food with ID {id} not found.");

            return await _foodRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error deleting food: {ex.Message}", ex);
        }
    }

    private FoodDto MapToDto(Food food)
    {
        return new FoodDto
        {
            Id = food.FoodId,
            Name = food.Name,
            Description = food.Description,
            Price = food.Price,
            DiscountType = food.DiscountType,
            Discount = food.Discount,
            Image = food.Image,
            CreatedAt = food.CreatedAt,
            UpdatedAt = food.UpdatedAt
        };
    }
}
