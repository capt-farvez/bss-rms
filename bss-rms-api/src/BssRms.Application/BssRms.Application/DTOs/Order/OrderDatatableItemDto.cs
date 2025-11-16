using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Order;

public class OrderDatatableItemDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("orderStatus")]
    public string OrderStatus { get; set; } = string.Empty;

    [JsonPropertyName("orderTime")]
    public DateTime OrderTime { get; set; }

    [JsonPropertyName("table")]
    public OrderTableInfoDto Table { get; set; } = null!;

    [JsonPropertyName("orderedBy")]
    public OrderUserInfoDto OrderedBy { get; set; } = null!;

    [JsonPropertyName("orderTakenBy")]
    public OrderUserInfoDto OrderTakenBy { get; set; } = null!;

    [JsonPropertyName("orderItems")]
    public List<OrderItemDetailDto> OrderItems { get; set; } = new();
}
