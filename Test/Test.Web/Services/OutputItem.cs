using System.Text.Json.Serialization;

namespace Test.Web;

internal record OutputItem
{
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("postedBy")]
    public string PostedBy { get; init; } = string.Empty;

    [JsonPropertyName("time")]
    [JsonConverter(typeof(Iso8601ToDateTimeOffsetConverter))]
    public DateTimeOffset Time { get; init; }

    [JsonPropertyName("score")]
    public int Score { get; init; }

    [JsonPropertyName("commentCount")]
    public int CommentCount { get; init; }
}