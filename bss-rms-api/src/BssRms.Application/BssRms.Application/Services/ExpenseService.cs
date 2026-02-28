using BssRms.Application.DTOs.Expense;
using BssRms.Application.Interfaces;
using BssRms.Domain.Entities;
using BssRms.Domain.Interfaces;

namespace BssRms.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto)
    {
        try
        {
            var expense = new Expense
            {
                Title = dto.Title,
                Category = dto.Category,
                Amount = dto.Amount,
                ExpenseDate = dto.ExpenseDate,
                Notes = dto.Notes
            };

            var created = await _expenseRepository.CreateAsync(expense);
            return MapToDto(created);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating expense: {ex.Message}", ex);
        }
    }

    public async Task<ExpenseDto?> GetByIdAsync(int id)
    {
        try
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
                return null;

            return MapToDto(expense);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving expense: {ex.Message}", ex);
        }
    }

    public async Task<ExpenseDatatableDto> GetDatatableAsync(int page, int perPage, string? search, string? sort)
    {
        try
        {
            var (data, totalRecords) = await _expenseRepository.GetDatatableAsync(page, perPage, search, sort);
            var lastPage = (int)Math.Ceiling((double)totalRecords / perPage);

            return new ExpenseDatatableDto
            {
                Data = data.Select(e => new ExpenseDatatableItemDto
                {
                    Id = e.ExpenseId,
                    Title = e.Title,
                    Category = e.Category,
                    Amount = e.Amount,
                    ExpenseDate = e.ExpenseDate,
                    Notes = e.Notes
                }).ToList(),
                CurrentPage = page,
                PerPage = perPage,
                Total = totalRecords,
                LastPage = lastPage
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving expense datatable: {ex.Message}", ex);
        }
    }

    public async Task<ExpenseDto> UpdateAsync(int id, UpdateExpenseDto dto)
    {
        try
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
                throw new KeyNotFoundException($"Expense with ID {id} not found.");

            expense.Title = dto.Title;
            expense.Category = dto.Category;
            expense.Amount = dto.Amount;
            expense.ExpenseDate = dto.ExpenseDate;
            expense.Notes = dto.Notes;

            var updated = await _expenseRepository.UpdateAsync(expense);
            return MapToDto(updated);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error updating expense: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var exists = await _expenseRepository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"Expense with ID {id} not found.");

            return await _expenseRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw ex is KeyNotFoundException ? ex : new Exception($"Error deleting expense: {ex.Message}", ex);
        }
    }

    private ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.ExpenseId,
            Title = expense.Title,
            Category = expense.Category,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            Notes = expense.Notes,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
