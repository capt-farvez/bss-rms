using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BssRms.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        expense.CreatedAt = DateTime.UtcNow;
        expense.UpdatedAt = DateTime.UtcNow;

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return expense;
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        return await _context.Expenses.FindAsync(id);
    }

    public async Task<(List<Expense> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        IQueryable<Expense> query = _context.Expenses;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e =>
                e.Title.Contains(search) ||
                e.Category.Contains(search) ||
                (e.Notes != null && e.Notes.Contains(search)));
        }

        var totalRecords = await query.CountAsync();

        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort.ToLower() switch
            {
                "title" => query.OrderBy(e => e.Title),
                "-title" => query.OrderByDescending(e => e.Title),
                "amount" => query.OrderBy(e => e.Amount),
                "-amount" => query.OrderByDescending(e => e.Amount),
                "expensedate" => query.OrderBy(e => e.ExpenseDate),
                "-expensedate" => query.OrderByDescending(e => e.ExpenseDate),
                "category" => query.OrderBy(e => e.Category),
                "-category" => query.OrderByDescending(e => e.Category),
                "createdat" => query.OrderBy(e => e.CreatedAt),
                "-createdat" => query.OrderByDescending(e => e.CreatedAt),
                _ => query.OrderByDescending(e => e.ExpenseDate)
            };
        }
        else
        {
            query = query.OrderByDescending(e => e.ExpenseDate);
        }

        var data = await query
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        return (data, totalRecords);
    }

    public async Task<Expense> UpdateAsync(Expense expense)
    {
        expense.UpdatedAt = DateTime.UtcNow;
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();

        return expense;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
            return false;

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Expenses.AnyAsync(e => e.ExpenseId == id);
    }

    public async Task<decimal> GetTotalExpenseAmountAsync(DateTime? from, DateTime? to)
    {
        var query = _context.Expenses.AsQueryable();

        if (from.HasValue)
            query = query.Where(e => e.ExpenseDate >= from.Value);
        if (to.HasValue)
            query = query.Where(e => e.ExpenseDate < to.Value);

        return await query.Select(e => (decimal?)e.Amount).SumAsync() ?? 0m;
    }
}
