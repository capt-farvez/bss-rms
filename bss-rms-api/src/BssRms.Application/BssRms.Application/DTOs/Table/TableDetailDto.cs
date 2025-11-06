using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Table;

public class TableDetailDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("tableNumber")]
    public string TableNumber { get; set; } = string.Empty;

    [JsonPropertyName("numberOfSeats")]
    public int NumberOfSeats { get; set; }

    [JsonPropertyName("isOccupied")]
    public bool IsOccupied { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("employees")]
    public List<TableEmployeeDto> Employees { get; set; } = new();
}
