namespace InventoryService.Models;

public class ReserveInventoryResponse
{
    public Guid ProductId { get; set; }
    public bool Success { get; set; }
}