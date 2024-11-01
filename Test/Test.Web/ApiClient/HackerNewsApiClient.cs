namespace Test.Web;

internal class HackerNewsApiClient : IHackerNewsApiClient
{
    private HttpClient HttpClient { get; }

    public HackerNewsApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<IReadOnlyList<int>> GetBestStoriesAsync(CancellationToken cancellationToken)
    {
        var result = await HttpClient.GetFromJsonAsync<IReadOnlyList<int>>("v0/beststories.json", cancellationToken);
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    public async Task<HackerNewsItemModel> GetItemAsync(int id, CancellationToken cancellationToken)
    {
        var result = await HttpClient.GetFromJsonAsync<HackerNewsItemModel>($"v0/item/{id}.json", cancellationToken);
        ArgumentNullException.ThrowIfNull(result);
        return result;
    }
}