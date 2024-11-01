using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Test.Web;

public class Iso8601ToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private const string DateFormat = "yyyy-MM-ddTHH:mm:sszzz"; // Define the format

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string with 'yyyy-MM-ddTHH:mm:sszzz' format.");
        }

        var dateString = reader.GetString()!;
        return DateTimeOffset.ParseExact(dateString, DateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}