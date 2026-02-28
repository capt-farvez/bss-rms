using BssRms.Domain.Entities;

namespace BssRms.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<Expense> CreateAsync(Expense expense);
    Task<Expense?> GetByIdAsync(int id);
    Task<(List<Expense> Data, int TotalRecords)> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<Expense> UpdateAsync(Expense expense);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<decimal> GetTotalExpenseAmountAsync(DateTime? from, DateTime? to);
}
