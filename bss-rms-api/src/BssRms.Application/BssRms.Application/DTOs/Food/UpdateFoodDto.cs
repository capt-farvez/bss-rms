using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Food;

public class UpdateFoodDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("discountType")]
    [JsonConverter(typeof(DiscountTypeConverter))]
    public string DiscountType { get; set; } = "None";

    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("base64")]
    public string? Base64 { get; set; }
}
