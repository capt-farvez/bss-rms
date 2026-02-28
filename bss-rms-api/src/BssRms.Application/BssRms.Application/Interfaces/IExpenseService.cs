using BssRms.Application.DTOs.Expense;

namespace BssRms.Application.Interfaces;

public interface IExpenseService
{
    Task<ExpenseDto> CreateAsync(CreateExpenseDto dto);
    Task<ExpenseDto?> GetByIdAsync(int id);
    Task<ExpenseDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort);
    Task<ExpenseDto> UpdateAsync(int id, UpdateExpenseDto dto);
    Task<bool> DeleteAsync(int id);
}
