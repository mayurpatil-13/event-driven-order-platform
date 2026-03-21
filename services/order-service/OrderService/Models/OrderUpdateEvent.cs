using InventoryService.Models;
using OrderService.Models;

namespace OrderProcessorWorker.Models;

public class OrderUpdateEvent
{
    public Guid OrderId { get; set; }
    public required string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public bool IsSuccess { get; set; }
    public List<ReserveInventoryResponse> InventoryResults { get; set; } = new();   
}