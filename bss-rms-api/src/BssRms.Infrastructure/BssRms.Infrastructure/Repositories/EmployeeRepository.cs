using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        employee.EmployeeId = Guid.NewGuid();
        employee.CreatedAt = DateTime.UtcNow;
        employee.UpdatedAt = DateTime.UtcNow;

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.User)
            .ToListAsync();
    }

    public async Task<(List<Employee> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        IQueryable<Employee> query = _context.Employees.Include(e => e.User);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.User.FirstName.Contains(search) ||
                e.User.LastName.Contains(search) ||
                e.User.Email.Contains(search) ||
                e.User.PhoneNumber.Contains(search) ||
                e.Designation.Contains(search));
        }

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParts = sort.Split(' ');
            var sortField = sortParts[0];
            var sortDirection = sortParts.Length > 1 ? sortParts[1].ToLower() : "asc";

            query = sortField.ToLower() switch
            {
                "firstname" => sortDirection == "desc"
                    ? query.OrderByDescending(e => e.User.FirstName)
                    : query.OrderBy(e => e.User.FirstName),
                "lastname" => sortDirection == "desc"
                    ? query.OrderByDescending(e => e.User.LastName)
                    : query.OrderBy(e => e.User.LastName),
                "email" => sortDirection == "desc"
                    ? query.OrderByDescending(e => e.User.Email)
                    : query.OrderBy(e => e.User.Email),
                "designation" => sortDirection == "desc"
                    ? query.OrderByDescending(e => e.Designation)
                    : query.OrderBy(e => e.Designation),
                "joindate" => sortDirection == "desc"
                    ? query.OrderByDescending(e => e.JoinDate)
                    : query.OrderBy(e => e.JoinDate),
                _ => query.OrderByDescending(e => e.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(e => e.CreatedAt);
        }

        var data = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        return (data, totalRecords);
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        employee.UpdatedAt = DateTime.UtcNow;
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // SQL: DELETE FROM [Employee] WHERE EmployeeId = @id
        //      DELETE FROM [User] WHERE Uid = @userId

        var employee = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);

        if (employee == null)
            return false;

        // Delete the employee first (to avoid foreign key constraint issues)
        _context.Employees.Remove(employee);

        // Then delete the associated user if it exists
        if (employee.User != null)
        {
            _context.Users.Remove(employee.User);
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Employees.AnyAsync(e => e.EmployeeId == id);
    }

    public async Task<List<Employee>> GetNonAssignedEmployeesAsync(int tableId)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Where(e => !_context.EmployeeTables
                .Where(et => et.TableId == tableId)
                .Select(et => et.EmployeeId)
                .Contains(e.EmployeeId))
            .ToListAsync();
    }
}
