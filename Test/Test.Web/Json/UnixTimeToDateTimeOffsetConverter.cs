using System.Text.Json;
using System.Text.Json.Serialization;

namespace Test.Web;

public class UnixTimeToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Ensure the value is a valid number (Unix timestamp)
        if (reader.TokenType != JsonTokenType.Number || !reader.TryGetInt64(out long secondsSinceEpoch))
        {
            throw new JsonException("Expected Unix timestamp as a number.");
        }

        // Convert Unix timestamp to DateTimeOffset
        return DateTimeOffset.FromUnixTimeSeconds(secondsSinceEpoch);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // Convert DateTimeOffset to Unix timestamp
        long secondsSinceEpoch = value.ToUnixTimeSeconds();
        writer.WriteNumberValue(secondsSinceEpoch);
    }
}