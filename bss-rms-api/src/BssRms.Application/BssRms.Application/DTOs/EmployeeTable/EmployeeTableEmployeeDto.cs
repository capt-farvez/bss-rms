using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.EmployeeTable;

public class EmployeeTableEmployeeDto
{
    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
