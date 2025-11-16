using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Order;

public class OrderSimpleDto
{
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;
}
