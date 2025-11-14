using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.EmployeeTable;

public class EmployeeTableListDto
{
    [JsonPropertyName("employeeTableId")]
    public int EmployeeTableId { get; set; }

    [JsonPropertyName("employee")]
    public EmployeeTableEmployeeDto Employee { get; set; } = null!;

    [JsonPropertyName("table")]
    public EmployeeTableTableDto Table { get; set; } = null!;
}
