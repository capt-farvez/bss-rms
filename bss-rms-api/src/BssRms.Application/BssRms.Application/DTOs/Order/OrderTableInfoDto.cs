using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Order;

public class OrderTableInfoDto
{
    [JsonPropertyName("tableId")]
    public int TableId { get; set; }

    [JsonPropertyName("tableNumber")]
    public string TableNumber { get; set; } = string.Empty;
}
