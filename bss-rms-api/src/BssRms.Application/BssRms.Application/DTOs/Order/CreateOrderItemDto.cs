using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Order;

public class CreateOrderItemDto
{
    [JsonPropertyName("foodId")]
    public int FoodId { get; set; }

    [JsonPropertyName("foodPackageId")]
    public int? FoodPackageId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }
}
