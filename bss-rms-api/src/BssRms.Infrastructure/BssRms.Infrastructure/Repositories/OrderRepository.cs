using BssRms.Domain.Entities;
using BssRms.Domain.Enums;
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
        // SQL: SELECT * FROM [Order] o
        //      LEFT JOIN [Table] t ON o.TableId = t.TableId
        //      LEFT JOIN [Employee] eo ON o.OrderedById = eo.EmployeeId
        //      LEFT JOIN [User] ueo ON eo.UserId = ueo.Uid
        //      LEFT JOIN [Employee] et ON o.OrderTakenById = et.EmployeeId
        //      LEFT JOIN [User] uet ON et.UserId = uet.Uid
        //      LEFT JOIN [OrderItem] oi ON o.OrderId = oi.OrderId
        //      LEFT JOIN [Food] f ON oi.FoodId = f.FoodId
        //      WHERE o.OrderId = @id

        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderedBy)
                .ThenInclude(e => e.User)
            .Include(o => o.OrderTakenBy)
                .ThenInclude(e => e.User)
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

    public async Task<(List<Order> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        // SQL: SELECT * FROM [Order] o
        //      LEFT JOIN [Table] t ON o.TableId = t.TableId
        //      LEFT JOIN [Employee] eo ON o.OrderedById = eo.EmployeeId
        //      LEFT JOIN [User] ueo ON eo.UserId = ueo.Uid
        //      LEFT JOIN [Employee] et ON o.OrderTakenById = et.EmployeeId
        //      LEFT JOIN [User] uet ON et.UserId = uet.Uid
        //      LEFT JOIN [OrderItem] oi ON o.OrderId = oi.OrderId
        //      LEFT JOIN [Food] f ON oi.FoodId = f.FoodId
        //      WHERE o.OrderNumber LIKE '%@search%' OR t.TableNumber LIKE '%@search%'
        //      ORDER BY @sort
        //      OFFSET @skip ROWS FETCH NEXT @perPage ROWS ONLY

        IQueryable<Order> query = _context.Orders
            .Include(o => o.Table)
            .Include(o => o.OrderedBy)
                .ThenInclude(e => e.User)
            .Include(o => o.OrderTakenBy)
                .ThenInclude(e => e.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(o =>
                o.OrderNumber.Contains(search) ||
                o.Table.TableNumber.Contains(search) ||
                o.PhoneNumber.Contains(search));
        }

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort.ToLower() switch
            {
                "ordernumber" => query.OrderBy(o => o.OrderNumber),
                "-ordernumber" => query.OrderByDescending(o => o.OrderNumber),
                "amount" => query.OrderBy(o => o.Amount),
                "-amount" => query.OrderByDescending(o => o.Amount),
                "orderdate" => query.OrderBy(o => o.OrderDate),
                "-orderdate" => query.OrderByDescending(o => o.OrderDate),
                "createdat" => query.OrderBy(o => o.CreatedAt),
                "-createdat" => query.OrderByDescending(o => o.CreatedAt),
                _ => query.OrderBy(o => o.CreatedAt)
            };
        }
        else
        {
            query = query.OrderBy(o => o.CreatedAt);
        }

        var data = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
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

    public async Task<int> GetPaidOrderCountAsync(DateTime? from, DateTime? to)
    {
        var query = _context.Orders.Where(o => o.Status == (int)OrderStatus.Paid);

        if (from.HasValue)
            query = query.Where(o => o.OrderDate >= from.Value);
        if (to.HasValue)
            query = query.Where(o => o.OrderDate < to.Value);

        return await query.CountAsync();
    }

    public async Task<decimal> GetPaidOrderAmountAsync(DateTime? from, DateTime? to)
    {
        var query = _context.Orders.Where(o => o.Status == (int)OrderStatus.Paid);

        if (from.HasValue)
            query = query.Where(o => o.OrderDate >= from.Value);
        if (to.HasValue)
            query = query.Where(o => o.OrderDate < to.Value);

        return await query.Select(o => (decimal?)o.Amount).SumAsync() ?? 0m;
    }
}
