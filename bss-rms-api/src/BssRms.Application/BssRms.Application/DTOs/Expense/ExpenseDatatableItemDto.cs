using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Expense;

public class ExpenseDatatableItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("expenseDate")]
    public DateTime ExpenseDate { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}
