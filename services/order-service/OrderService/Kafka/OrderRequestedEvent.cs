namespace OrderService.Kafka
{
    public class OrderRequestedEvent
    {
        public Guid OrderId { get; set; }
        public required string CustomerId { get; set; }
        public List<OrderItemEvent> Items { get; set; } = new();
    }
}