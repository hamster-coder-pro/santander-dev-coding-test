namespace Test.Web;

internal interface ITestService
{
    Task<IReadOnlyList<OutputItem>> GetBestStoriesAsync(int limit = 1, CancellationToken cancellationToken = default);
}