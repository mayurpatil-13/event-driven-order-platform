using InventoryService.Data;
using InventoryService.Interfaces;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services;

public class ProductService : IProductService
{
    private readonly InventoryDbContext _context;
    public ProductService(InventoryDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AddProduct(Product product)
    {
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> GetProductById(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id)
         ?? throw new InvalidOperationException("Product not found");
    }

    public async Task UpdateProduct(Product product)
    {
        var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existingProduct == null)
            throw new InvalidOperationException("Product not found");

        existingProduct.ProductName = product.ProductName;
        existingProduct.Price = product.Price;
        existingProduct.Stock = product.Stock;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();
    }
}