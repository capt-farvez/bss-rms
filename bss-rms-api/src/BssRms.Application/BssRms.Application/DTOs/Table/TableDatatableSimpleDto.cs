using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Table;

public class TableDatatableSimpleDto
{
    [JsonPropertyName("data")]
    public List<TableDetailDto> Data { get; set; } = new();

    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }
}
