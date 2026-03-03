using Microsoft.EntityFrameworkCore;
using InventoryService.Models;

namespace Inventory.Kafka
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public List<OrderItemEvent> Items { get; set; } = new();
    }

    public class OrderItemEvent
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}