using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class TableRepository : ITableRepository
{
    private readonly ApplicationDbContext _context;

    public TableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Table> CreateAsync(Table table)
    {
        table.CreatedAt = DateTime.UtcNow;
        table.UpdatedAt = DateTime.UtcNow;

        await _context.Tables.AddAsync(table);
        await _context.SaveChangesAsync();

        return table;
    }

    public async Task<Table?> GetByIdAsync(int id)
    {
        return await _context.Tables
            .Include(t => t.EmployeeTables)
                .ThenInclude(et => et.Employee)
                    .ThenInclude(e => e.User)
            .Include(t => t.Orders)
            .FirstOrDefaultAsync(t => t.TableId == id);
    }

    public async Task<List<Table>> GetAllAsync()
    {
        return await _context.Tables.ToListAsync();
    }

    public async Task<(List<Table> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        // SQL: SELECT t.*, et.*, e.*, u.*, o.*
        //      FROM [Table] t
        //      LEFT JOIN [EmployeeTable] et ON t.TableId = et.TableId
        //      LEFT JOIN [Employee] e ON et.EmployeeId = e.EmployeeId
        //      LEFT JOIN [User] u ON e.UserId = u.Uid
        //      LEFT JOIN [Order] o ON t.TableId = o.TableId
        //      WHERE t.TableNumber LIKE '%@search%'
        //      ORDER BY t.CreatedAt DESC
        //      OFFSET @skip ROWS FETCH NEXT @perPage ROWS ONLY

        IQueryable<Table> query = _context.Tables
            .Include(t => t.EmployeeTables)              // Load assigned employees (junction table)
                .ThenInclude(et => et.Employee)          // Load employee details for each assignment
                    .ThenInclude(e => e.User)            // Load user info (name, email) for each employee
            .Include(t => t.Orders);                     // Load orders to check if table is occupied

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.TableNumber.Contains(search));
        }

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort.ToLower() switch
            {
                "tablenumber" => query.OrderBy(t => t.TableNumber),
                "-tablenumber" => query.OrderByDescending(t => t.TableNumber),
                "numberofseats" => query.OrderBy(t => t.NumberOfSeats),
                "-numberofseats" => query.OrderByDescending(t => t.NumberOfSeats),
                "createdat" => query.OrderBy(t => t.CreatedAt),
                "-createdat" => query.OrderByDescending(t => t.CreatedAt),
                _ => query.OrderByDescending(t => t.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(t => t.CreatedAt);
        }

        var data = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        return (data, totalRecords);
    }

    public async Task<Table> UpdateAsync(Table table)
    {
        table.UpdatedAt = DateTime.UtcNow;

        _context.Tables.Update(table);
        await _context.SaveChangesAsync();

        return table;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null)
            return false;

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByTableNumberAsync(string tableNumber, int? excludeId = null)
    {
        var query = _context.Tables.Where(t => t.TableNumber == tableNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(t => t.TableId != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
