using Microsoft.AspNetCore.Mvc;
using Test.Web;

var builder = WebApplication.CreateBuilder(args);

// add typed http client 
builder.Services.AddHttpClient<IHackerNewsApiClient, HackerNewsApiClient>(configure =>
{
    configure.BaseAddress = new Uri("https://hacker-news.firebaseio.com/", UriKind.Absolute);
});

// add memory cache for demo reasons
builder.Services.AddMemoryCache(options => { options.TrackStatistics = true; });

// 
builder.Services.AddTransient<ITestService, TestService>();
builder.Services.Decorate<ITestService, CachedTestService>();

// add cache abstraction interfaces and class
builder.Services.AddSingleton<AppCache>();
builder.Services.AddTransient<ICacheUpdater>(sp => sp.GetRequiredService<AppCache>());
builder.Services.AddTransient<ICacheProvider>(sp => sp.GetRequiredService<AppCache>());

// add cache timeout settings
builder.Services.AddOptions<CacheSettings>().Configure((CacheSettings settings, IConfiguration configuration) =>
{
    settings.Timeout = TimeSpan.Parse(configuration.GetRequiredSection("cache:timeout").Value ?? string.Empty);
});

var app = builder.Build();

app.MapGet("/", (HttpContext httpContext) => Results.LocalRedirect("/beststories"));

app.MapGet("/beststories", BestStories);

app.Run();

static async Task<IResult> BestStories([FromServices] ITestService testService, HttpContext httpContext, [FromQuery] int limit = 1)
{
    if (limit < 1 || limit > 500)
    {
        return Results.BadRequest($"Require {nameof(limit)} value between 1 and 500");
    }
    var result = await testService.GetBestStoriesAsync(limit, httpContext.RequestAborted);
    return Results.Ok(result);
}