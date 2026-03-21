namespace InventoryService.Models;

public class ReserveInventoryRequest
{
    public Guid OrderId { get; set; }
    public List<InventoryItem> Items { get; set; } = new();
}

public class InventoryItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}