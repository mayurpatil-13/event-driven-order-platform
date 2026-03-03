using InventoryService.Models;

namespace InventoryService.Interfaces
{
    public interface IProductService
    {
        Task AddProduct(Product product);
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProductById(Guid id);
        Task UpdateProduct(Product product);
    }
}