using System.Net.Http.Json;
using OrderProcessorWorker.Models;

namespace OrderProcessorWorker.Clients;

public class InventoryClient
{
    private readonly HttpClient _httpClient;

    public InventoryClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ReserveInventoryResponse>> ReserveInventoryAsync(ReserveInventoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5238/api/inventory/reserve", request);

        response.EnsureSuccessStatusCode();

        var responses = await response.Content.ReadFromJsonAsync<List<ReserveInventoryResponse>>();
        return responses ?? new List<ReserveInventoryResponse>();
    }
}