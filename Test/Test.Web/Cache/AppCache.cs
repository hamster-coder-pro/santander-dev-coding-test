using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Test.Web;

internal class AppCache : ICacheProvider, ICacheUpdater
{
    private IMemoryCache MemoryCache { get; }

    private CacheSettings CacheSettings { get; }

    public AppCache(IMemoryCache memoryCache, IOptions<CacheSettings> cacheSettingsOptions)
    {
        MemoryCache   = memoryCache;
        CacheSettings = cacheSettingsOptions.Value;
    }

    public async Task<IReadOnlyList<OutputItem>> GetResultAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        if (MemoryCache.TryGetValue<IReadOnlyList<OutputItem>>("result", out var result))
        {
            return result!;
        }

        return Array.Empty<OutputItem>();
    }

    public Task SetResultAsync(IReadOnlyList<OutputItem> result, CancellationToken cancellationToken = default)
    {
        using var entry = MemoryCache.CreateEntry("result");
        entry.Value                           = result;
        entry.AbsoluteExpirationRelativeToNow = CacheSettings.Timeout;
        return Task.CompletedTask;
    }
}