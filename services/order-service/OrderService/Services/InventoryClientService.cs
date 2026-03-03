using OrderService.Interfaces;

namespace OrderService.Services;

public class InventoryClientService : IInventoryClient
{
    private readonly HttpClient _httpClient;

    public InventoryClientService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ProductResponse> GetProduct(Guid productId)
    {
        var response = await _httpClient.GetAsync($"http://localhost:5238/api/products/{productId}");
        response.EnsureSuccessStatusCode();

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        return product ?? throw new InvalidOperationException("Product not found");
    }
}