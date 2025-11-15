using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Food;

public class FoodSimpleDto
{
    [JsonPropertyName("foodId")]
    public int FoodId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
