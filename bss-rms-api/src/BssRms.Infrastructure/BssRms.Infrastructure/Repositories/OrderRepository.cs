using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        order.OrderDate = DateTime.UtcNow;
        order.Status = 0; // Default status: Pending

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {

        return await _context.Orders
            .AsSplitQuery()
            .Include(o => o.Table)
            .Include(o => o.OrderedBy)
                .ThenInclude(e => e!.User)
            .Include(o => o.OrderTakenBy)
                .ThenInclude(e => e!.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<(List<Order> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort, int? status)
    {
        // Base query without Includes — used for counting
        IQueryable<Order> baseQuery = _context.Orders.AsQueryable();

        if (status.HasValue)
        {
            baseQuery = baseQuery.Where(o => o.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            baseQuery = baseQuery.Where(o =>
                o.OrderNumber.Contains(search) ||
                o.Table.TableNumber.Contains(search) ||
                (o.PhoneNumber != null && o.PhoneNumber.Contains(search)));
        }

        // Count on lightweight query (no JOINs)
        var totalRecords = await baseQuery.CountAsync();

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sort))
        {
            baseQuery = sort.ToLower() switch
            {
                "ordernumber" => baseQuery.OrderBy(o => o.OrderNumber),
                "-ordernumber" => baseQuery.OrderByDescending(o => o.OrderNumber),
                "amount" => baseQuery.OrderBy(o => o.Amount),
                "-amount" => baseQuery.OrderByDescending(o => o.Amount),
                "orderdate" => baseQuery.OrderBy(o => o.OrderDate),
                "-orderdate" => baseQuery.OrderByDescending(o => o.OrderDate),
                "createdat" => baseQuery.OrderBy(o => o.CreatedAt),
                "-createdat" => baseQuery.OrderByDescending(o => o.CreatedAt),
                "status" => baseQuery.OrderBy(o => o.Status),
                "-status" => baseQuery.OrderByDescending(o => o.Status),
                _ => baseQuery.OrderBy(o => o.CreatedAt)
            };
        }
        else
        {
            baseQuery = baseQuery.OrderBy(o => o.CreatedAt);
        }

        // Add Includes only for the paginated data fetch
        var data = await baseQuery
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .AsSplitQuery()
            .Include(o => o.Table)
            .Include(o => o.OrderedBy)
                .ThenInclude(e => e!.User)
            .Include(o => o.OrderTakenBy)
                .ThenInclude(e => e!.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
            .ToListAsync();

        return (data, totalRecords);
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null){
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Orders.AnyAsync(o => o.OrderId == id);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Orders.CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _context.Orders.SumAsync(o => o.Amount);
    }

    public async Task<List<Order>> GetRecentOrdersAsync(int count)
    {
        return await _context.Orders
            .Include(o => o.Table)
            .OrderByDescending(o => o.OrderDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<(int FoodId, string FoodName, decimal FoodPrice, string FoodImage, int TotalQuantity, decimal TotalRevenue)>> GetTopSellingFoodsAsync(int count)
    {
        return await _context.Set<OrderItem>()
            .Include(oi => oi.Food)
            .GroupBy(oi => new { oi.FoodId, oi.Food.Name, oi.Food.Price, oi.Food.Image })
            .Select(g => new
            {
                g.Key.FoodId,
                g.Key.Name,
                g.Key.Price,
                g.Key.Image,
                TotalQuantity = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => oi.TotalPrice)
            })
            .OrderByDescending(x => x.TotalQuantity)
            .Take(count)
            .Select(x => ValueTuple.Create(x.FoodId, x.Name, x.Price, x.Image, x.TotalQuantity, x.TotalRevenue))
            .ToListAsync();
    }

    public async Task<int> GetTodaysOrderCountAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Orders.CountAsync(o => o.OrderDate.Date == today);
    }

    public async Task<decimal> GetTodaysRevenueAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Orders
            .Where(o => o.OrderDate.Date == today)
            .SumAsync(o => o.Amount);
    }
}
