using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.EmployeeTable;

public class UpdateEmployeeTableDto
{
    [JsonPropertyName("employeeId")]
    public Guid EmployeeId { get; set; }

    [JsonPropertyName("tableId")]
    public int TableId { get; set; }
}
