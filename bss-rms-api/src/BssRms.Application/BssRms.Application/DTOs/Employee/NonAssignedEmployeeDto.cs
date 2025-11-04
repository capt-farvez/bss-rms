using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Employee;

public class NonAssignedEmployeeDto
{
    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
