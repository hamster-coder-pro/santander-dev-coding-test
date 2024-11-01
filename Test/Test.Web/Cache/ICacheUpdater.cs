namespace Test.Web;

internal interface ICacheUpdater
{
    Task SetResultAsync(IReadOnlyList<OutputItem> result, CancellationToken cancellationToken = default);
}