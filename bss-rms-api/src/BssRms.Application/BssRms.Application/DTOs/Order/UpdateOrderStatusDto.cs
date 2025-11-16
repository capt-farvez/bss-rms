using System.Text.Json;
using System.Text.Json.Serialization;

namespace BssRms.Application.DTOs.Order;

public class UpdateOrderStatusDto
{
    [JsonPropertyName("status")]
    [JsonConverter(typeof(OrderStatusConverter))]
    public string Status { get; set; } = "0"; // Default to Pending (0)
}

public class OrderStatusConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            var value = reader.GetInt32();
            return value switch
            {
                0 => "0", // Pending
                1 => "1", // Confirmed
                2 => "2", // Preparing
                3 => "3", // PreparedToServe
                4 => "4", // Served
                5 => "5", // Paid
                _ => "0"  // Default to Pending
            };
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() ?? "0";
        }

        return "0";
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
