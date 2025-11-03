using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Employee;

public class EmployeeDatatableDto
{
    public List<EmployeeDto> Data { get; set; } = new();

    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    public int Total { get; set; }

    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }
}
