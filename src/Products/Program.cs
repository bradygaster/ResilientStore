using Bogus;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationMonitoring();
builder.Services.AddMemoryCache();

var app = builder.Build();

// generate the list of products
var products = new Faker<Product>()
    .StrictMode(true)
    .RuleFor(p => p.ProductId, (f, p) => f.Database.Random.Guid())
    .RuleFor(p => p.ProductName, (f, p) => f.Commerce.ProductName()).Generate(10);

// error counter, we want to fail every 3rd hit
var counter = 0;

// mapget for all the products
app.MapGet("/products", () =>
{
    if(counter <= 2)
    {
        counter++;
        return Results.Ok(products);
    }
    else
    {
        counter = 0;
        throw new ApplicationException("Random error occurred!");
    }
});

app.MapGet("/", () => "Products API");

app.Run();

public class Product
{
    public Guid ProductId { get; set; } = Guid.NewGuid();
    public string ProductName { get; set; } = string.Empty;
}
