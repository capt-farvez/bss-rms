using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class FoodRepository : IFoodRepository
{
    private readonly ApplicationDbContext _context;

    public FoodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Food> CreateAsync(Food food)
    {
        food.CreatedAt = DateTime.UtcNow;
        food.UpdatedAt = DateTime.UtcNow;

        _context.Foods.Add(food);
        await _context.SaveChangesAsync();

        return food;
    }

    public async Task<Food?> GetByIdAsync(int id)
    {
        return await _context.Foods.FindAsync(id);
    }

    public async Task<List<Food>> GetAllAsync()
    {
        return await _context.Foods
            .OrderBy(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<(List<Food> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        IQueryable<Food> query = _context.Foods;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(f =>
                f.Name.Contains(search) ||
                f.Description.Contains(search));
        }

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort.ToLower() switch
            {
                "name" => query.OrderBy(f => f.Name),
                "-name" => query.OrderByDescending(f => f.Name),
                "price" => query.OrderBy(f => f.Price),
                "-price" => query.OrderByDescending(f => f.Price),
                "createdat" => query.OrderBy(f => f.CreatedAt),
                "-createdat" => query.OrderByDescending(f => f.CreatedAt),
                _ => query.OrderBy(f => f.CreatedAt)
            };
        }
        else
        {
            query = query.OrderBy(f => f.CreatedAt);
        }

        var data = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        return (data, totalRecords);
    }

    public async Task<Food> UpdateAsync(Food food)
    {
        food.UpdatedAt = DateTime.UtcNow;
        _context.Foods.Update(food);
        await _context.SaveChangesAsync();

        return food;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var food = await _context.Foods.FindAsync(id);
        if (food == null)
            return false;

        _context.Foods.Remove(food);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Foods.AnyAsync(f => f.FoodId == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.Foods.Where(f => f.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(f => f.FoodId != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
