namespace Test.Web;

internal interface ICacheProvider
{
    Task<IReadOnlyList<OutputItem>> GetResultAsync(CancellationToken cancellationToken = default);
}