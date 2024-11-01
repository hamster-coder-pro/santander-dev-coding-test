namespace Test.Web;

internal sealed class TestService : ITestService
{
    private IHackerNewsApiClient ApiClient { get; }

    public TestService(IHackerNewsApiClient apiClient)
    {
        ApiClient = apiClient;
    }

    public async Task<IReadOnlyList<OutputItem>> GetBestStoriesAsync(int limit, CancellationToken cancellationToken)
    {
        if (limit < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), limit, "Value must be greater than 0.");
        }

        var bestStories = await ApiClient.GetBestStoriesAsync(cancellationToken);
        var itemTasks   = bestStories.Take(limit).Select(id => GetItemAsync(id, cancellationToken));
        var items       = (await Task.WhenAll(itemTasks)).ToDictionary(x => x.Id, x => x);

        var result = bestStories.Take(limit).Select(id => MapToOutput(items[id])).ToArray();
        return result;
    }

    private OutputItem MapToOutput(HackerNewsItemModel source)
    {
        return new OutputItem
        {
            Title        = source.Title,
            Url          = source.Url,
            PostedBy     = source.PostedBy,
            Time         = source.Time,
            Score        = source.Score,
            CommentCount = source.CommentCount
        };
    }

    public Task<HackerNewsItemModel> GetItemAsync(int id, CancellationToken cancellationToken)
    {
        return ApiClient.GetItemAsync(id, cancellationToken);
    }
}