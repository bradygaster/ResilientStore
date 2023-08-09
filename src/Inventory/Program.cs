using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationMonitoring();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.MapGet("/inventory/{productId}", (string productId, IMemoryCache memoryCache) =>
{
    var memCacheKey = $"{productId}-inventory";
    int inventoryValue = -404;

    if (!memoryCache.TryGetValue(memCacheKey, out inventoryValue))
    {
        inventoryValue = new Random().Next(1, 100);
        memoryCache.Set(memCacheKey, inventoryValue);
    }

    inventoryValue = memoryCache.Get<int>(memCacheKey);

    return Results.Ok(inventoryValue);
});

app.MapGet("/", () => "Hello World!");

app.Run();
