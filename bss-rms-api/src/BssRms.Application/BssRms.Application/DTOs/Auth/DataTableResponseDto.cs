using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Auth;

public class DataTableResponseDto<T>
{
    [JsonPropertyName("data")]
    public IEnumerable<T> Data { get; set; } = new List<T>();

    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }
}
