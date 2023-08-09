using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IStoreBackendClient _storeBackendClient;
        public List<Product> Products { get; set; } = new();

        public IndexModel(ILogger<IndexModel> logger, IStoreBackendClient storeBackendClient)
        {
            _logger = logger;
            _storeBackendClient = storeBackendClient;
        }

        public async Task OnGet()
        {
            var products = await _storeBackendClient.GetProducts();
            foreach (var product in products)
            {
                product.Quantity = await _storeBackendClient.GetInventory(product.ProductId);
            }

            Products = products.ToList();
        }
    }
}
