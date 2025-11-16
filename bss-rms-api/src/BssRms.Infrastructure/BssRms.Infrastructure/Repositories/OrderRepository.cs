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
        // SQL: DELETE FROM [OrderItem] WHERE OrderId = @id
        //      DELETE FROM [Order] WHERE OrderId = @id

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
}
