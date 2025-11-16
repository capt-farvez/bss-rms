using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Order;

public class CreateOrderDto
{
    [JsonPropertyName("tableId")]
    public int TableId { get; set; }

    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("items")]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
