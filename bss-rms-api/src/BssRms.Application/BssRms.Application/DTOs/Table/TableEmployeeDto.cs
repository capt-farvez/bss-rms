using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Table;

public class TableEmployeeDto
{
    [JsonPropertyName("employeeTableId")]
    public int EmployeeTableId { get; set; }

    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
