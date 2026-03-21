using InventoryService.Data;
using InventoryService.Interfaces;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services;

public class InventoryService: IInventoryService
{
    private readonly InventoryDbContext _dbContext;

    public InventoryService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<List<ReserveInventoryResponse>>ReserveStockAsync(ReserveInventoryRequest request)
    {
        var InventoryItem = request.Items;
        var responses = new List<ReserveInventoryResponse>();
        if(InventoryItem == null || InventoryItem.Count == 0)
        {
            throw new ArgumentException("No items to reserve");
        }

        foreach(var item in InventoryItem)
        {
            var productExists = await _dbContext.Products
                .AnyAsync(p => p.Id == item.ProductId);
            if(!productExists)
            {
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
            }

            var rows = await _dbContext.Database.ExecuteSqlRawAsync(
                @"UPDATE Products
                  SET Stock = Stock - {0}
                  WHERE Id = {1} AND Stock >= {0}",
                item.Quantity,
                item.ProductId);

            if(rows == 0)
            {
                responses.Add(new ReserveInventoryResponse
                {
                    ProductId = item.ProductId,
                    Success = false
                });
                continue;
            }
            
            responses.Add(new ReserveInventoryResponse
            {
                ProductId = item.ProductId,
                Success = true
            });
        }

        await _dbContext.SaveChangesAsync();
        return responses;
    }

}