using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Food;

public class FoodDatatableItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("discountType")]
    public string DiscountType { get; set; } = "None";

    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    [JsonPropertyName("discountPrice")]
    public decimal DiscountPrice { get; set; }

    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;
}
