using InventoryService.Models;

namespace InventoryService.Interfaces;

public interface IInventoryService
{
    Task<List<ReserveInventoryResponse>>ReserveStockAsync(ReserveInventoryRequest request);
}