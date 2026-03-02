namespace OrderService.Interfaces;

public interface IInventoryClient
{
    Task<ProductResponse> GetProduct(Guid productId);
}

public class ProductResponse
{
    public Guid ProductId { get; set; } = Guid.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
