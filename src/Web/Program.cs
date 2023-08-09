using Refit;

var builder = WebApplication.CreateBuilder(args);

var productsUrl = string.IsNullOrEmpty(builder.Configuration.GetValue<string>("ProductsApi")) 
    ? "http://products" 
    : builder.Configuration.GetValue<string>("ProductsApi");

var inventoryUrl = string.IsNullOrEmpty(builder.Configuration.GetValue<string>("InventoryApi"))
    ? "http://inventory"
    : builder.Configuration.GetValue<string>("InventoryApi");

builder.Services.AddHttpClient("Products", (httpClient) => httpClient.BaseAddress = new Uri(productsUrl));

builder.Services.AddHttpClient("Inventory", (httpClient) => httpClient.BaseAddress = new Uri(inventoryUrl));

builder.Services.AddScoped<IStoreBackendClient, StoreBackendClient>();
builder.Services.AddApplicationMonitoring();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
app.Run();

public class Product
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public interface IStoreBackendClient
{
    [Get("/products")]
    Task<List<Product>> GetProducts();

    [Get("/inventory/{productId}")]
    Task<int> GetInventory(string productId);
}

public class StoreBackendClient : IStoreBackendClient
{
    IHttpClientFactory _httpClientFactory;

    public StoreBackendClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<int> GetInventory(string productId)
    {
        var client = _httpClientFactory.CreateClient("Inventory");
        return await RestService.For<IStoreBackendClient>(client).GetInventory(productId);
    }

    public async Task<List<Product>> GetProducts()
    {
        var client = _httpClientFactory.CreateClient("Products");
        return await RestService.For<IStoreBackendClient>(client).GetProducts();
    }
}
