using System.Text.Json;
using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Food;

public class CreateFoodDto
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
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("base64")]
    public string Base64 { get; set; } = string.Empty;
}

public class DiscountTypeConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var value = reader.GetInt32();
            return value switch
            {
                1 => "Percentage",
                2 => "Flat",
                _ => "None"
            };
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() ?? "None";
        }

        return "None";
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
