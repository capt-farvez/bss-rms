using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class EmployeeTableRepository : IEmployeeTableRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeTableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeTable> CreateAsync(EmployeeTable employeeTable)
    {
        employeeTable.CreatedAt = DateTime.UtcNow;
        employeeTable.UpdatedAt = DateTime.UtcNow;

        await _context.EmployeeTables.AddAsync(employeeTable);
        await _context.SaveChangesAsync();

        return employeeTable;
    }

    public async Task<List<EmployeeTable>> CreateRangeAsync(List<EmployeeTable> employeeTables)
    {
        // SQL: INSERT INTO [EmployeeTable] (EmployeeId, TableId, CreatedAt, UpdatedAt)
        //      VALUES
        //          (@employeeId1, @tableId1, GETUTCDATE(), GETUTCDATE()),
        //          (@employeeId2, @tableId2, GETUTCDATE(), GETUTCDATE()),
        //          ...

        var now = DateTime.UtcNow;
        foreach (var et in employeeTables)
        {
            et.CreatedAt = now;
            et.UpdatedAt = now;
        }

        await _context.EmployeeTables.AddRangeAsync(employeeTables);
        await _context.SaveChangesAsync();

        return employeeTables;
    }

    public async Task<EmployeeTable?> GetByIdAsync(int id)
    {
        // SQL: SELECT et.*, e.*, u.*, t.*
        //      FROM [EmployeeTable] et
        //      INNER JOIN [Employee] e ON et.EmployeeId = e.EmployeeId
        //      INNER JOIN [User] u ON e.UserId = u.Uid
        //      INNER JOIN [Table] t ON et.TableId = t.TableId
        //      WHERE et.EmployeeTableId = @id

        return await _context.EmployeeTables
            .Include(et => et.Employee)
                .ThenInclude(e => e.User)
            .Include(et => et.Table)
            .FirstOrDefaultAsync(et => et.EmployeeTableId == id);
    }

    public async Task<List<EmployeeTable>> GetAllAsync()
    {
        // SQL: SELECT et.*, e.*, u.*, t.*
        //      FROM [EmployeeTable] et
        //      INNER JOIN [Employee] e ON et.EmployeeId = e.EmployeeId
        //      INNER JOIN [User] u ON e.UserId = u.Uid
        //      INNER JOIN [Table] t ON et.TableId = t.TableId
        //      ORDER BY et.EmployeeTableId DESC

        return await _context.EmployeeTables
            .Include(et => et.Employee)
                .ThenInclude(e => e.User)
            .Include(et => et.Table)
            .OrderByDescending(et => et.EmployeeTableId)
            .ToListAsync();
    }

    public async Task<(List<EmployeeTable> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        // SQL: SELECT et.*, e.*, u.*, t.*
        //      FROM [EmployeeTable] et
        //      INNER JOIN [Employee] e ON et.EmployeeId = e.EmployeeId
        //      INNER JOIN [User] u ON e.UserId = u.Uid
        //      INNER JOIN [Table] t ON et.TableId = t.TableId
        //      WHERE u.FirstName LIKE '%@search%' OR u.LastName LIKE '%@search%' OR t.TableNumber LIKE '%@search%'
        //      ORDER BY et.CreatedAt DESC
        //      OFFSET @skip ROWS FETCH NEXT @perPage ROWS ONLY

        IQueryable<EmployeeTable> query = _context.EmployeeTables
            .Include(et => et.Employee)
                .ThenInclude(e => e.User)
            .Include(et => et.Table);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(et =>
                et.Employee.User.FirstName.Contains(search) ||
                et.Employee.User.LastName.Contains(search) ||
                et.Table.TableNumber.Contains(search));
        }

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort.ToLower() switch
            {
                "employeename" => query.OrderBy(et => et.Employee.User.FirstName),
                "-employeename" => query.OrderByDescending(et => et.Employee.User.FirstName),
                "tablenumber" => query.OrderBy(et => et.Table.TableNumber),
                "-tablenumber" => query.OrderByDescending(et => et.Table.TableNumber),
                "createdat" => query.OrderBy(et => et.CreatedAt),
                "-createdat" => query.OrderByDescending(et => et.CreatedAt),
                _ => query.OrderBy(et => et.CreatedAt)
            };
        }
        else
        {
            query = query.OrderBy(et => et.CreatedAt);
        }

        var data = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        return (data, totalRecords);
    }

    public async Task<EmployeeTable> UpdateAsync(EmployeeTable employeeTable)
    {
        // SQL: UPDATE [EmployeeTable]
        //      SET EmployeeId = @employeeId, TableId = @tableId, UpdatedAt = GETUTCDATE()
        //      WHERE EmployeeTableId = @id

        employeeTable.UpdatedAt = DateTime.UtcNow;

        _context.EmployeeTables.Update(employeeTable);
        await _context.SaveChangesAsync();

        return employeeTable;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // SQL: DELETE FROM [EmployeeTable]
        //      WHERE EmployeeTableId = @id

        var employeeTable = await _context.EmployeeTables.FindAsync(id);
        if (employeeTable == null)
        {
            return false;
        }

        _context.EmployeeTables.Remove(employeeTable);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(Guid employeeId, int tableId)
    {
        // SQL: SELECT CASE WHEN EXISTS (
        //          SELECT 1 FROM [EmployeeTable]
        //          WHERE EmployeeId = @employeeId AND TableId = @tableId
        //      ) THEN 1 ELSE 0 END

        return await _context.EmployeeTables
            .AnyAsync(et => et.EmployeeId == employeeId && et.TableId == tableId);
    }

    public async Task<bool> ExistsAsync(Guid employeeId, int tableId, int excludeId)
    {
        // SQL: SELECT CASE WHEN EXISTS (
        //          SELECT 1 FROM [EmployeeTable]
        //          WHERE EmployeeId = @employeeId AND TableId = @tableId AND EmployeeTableId != @excludeId
        //      ) THEN 1 ELSE 0 END

        return await _context.EmployeeTables
            .AnyAsync(et => et.EmployeeId == employeeId && et.TableId == tableId && et.EmployeeTableId != excludeId);
    }
}
