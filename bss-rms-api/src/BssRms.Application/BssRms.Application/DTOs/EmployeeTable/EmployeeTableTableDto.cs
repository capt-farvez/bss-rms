using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.EmployeeTable;

public class EmployeeTableTableDto
{
    [JsonPropertyName("tableId")]
    public int TableId { get; set; }

    [JsonPropertyName("tableNumber")]
    public string TableNumber { get; set; } = string.Empty;
}
