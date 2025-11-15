using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Food;

public class FoodDatatableDto
{
    [JsonPropertyName("data")]
    public List<FoodDatatableItemDto> Data { get; set; } = new();

    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }
}
