using System.Text.Json.Serialization;
using BssRms.Application.DTOs.Food;

namespace BssRms.Application.DTOs.Order;

public class OrderItemDetailDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("food")]
    public FoodDatatableItemDto Food { get; set; } = null!;
}
