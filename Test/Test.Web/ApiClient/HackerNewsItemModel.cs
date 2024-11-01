using System.Text.Json.Serialization;

namespace Test.Web;

internal class HackerNewsItemModel
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("by")]
    public string PostedBy { get; init; } = string.Empty;

    [JsonPropertyName("time")]
    [JsonConverter(typeof(UnixTimeToDateTimeOffsetConverter))]
    public DateTimeOffset Time { get; init; }

    [JsonPropertyName("score")]
    public int Score { get; init; }

    [JsonPropertyName("descendants")]
    public int CommentCount { get; init; }
}