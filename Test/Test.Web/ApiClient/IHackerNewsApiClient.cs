namespace Test.Web;

internal interface IHackerNewsApiClient
{
    Task<IReadOnlyList<int>> GetBestStoriesAsync(CancellationToken cancellationToken);

    Task<HackerNewsItemModel> GetItemAsync(int id, CancellationToken cancellationToken);
}