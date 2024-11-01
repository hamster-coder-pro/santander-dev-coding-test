using Microsoft.Extensions.Options;

namespace Test.Web;

internal sealed class CachedTestService : ITestService
{
    private ITestService InnerService { get; }

    private IHackerNewsApiClient ApiClient { get; }

    private ICacheProvider CacheProvider { get; }

    private ICacheUpdater CacheUpdater { get; }

    private CacheSettings CacheSettings { get; }

    public CachedTestService(
        ITestService                    innerService,
        IHackerNewsApiClient            apiClient,
        ICacheProvider                  cacheProvider,
        ICacheUpdater                   cacheUpdater,
        IOptionsSnapshot<CacheSettings> cacheSettingsSnapshot
    )
    {
        InnerService  = innerService;
        ApiClient     = apiClient;
        CacheProvider = cacheProvider;
        CacheUpdater  = cacheUpdater;
        CacheSettings = cacheSettingsSnapshot.Value;
    }

    private SemaphoreSlim LockIdList { get; } = new(1, 1);

    public async Task<IReadOnlyList<OutputItem>> GetBestStoriesAsync(int limit = 1, CancellationToken cancellationToken = default)
    {
        var result = await CacheProvider.GetResultAsync(cancellationToken);
        if (result.Count < limit)
        {
            await LockIdList.WaitAsync(cancellationToken);
            try
            {
                result = await CacheProvider.GetResultAsync(cancellationToken);
                if (result.Count < limit)
                {
                    result = await InnerService.GetBestStoriesAsync(limit, cancellationToken);
                    await CacheUpdater.SetResultAsync(result, cancellationToken);
                }
            }
            finally
            {
                LockIdList.Release();
            }
        }

        if (result.Count == limit)
        {
            return result;
        }

        return result.Take(limit).ToArray();
    }
}