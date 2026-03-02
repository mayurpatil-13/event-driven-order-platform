namespace OrderService.Kafka
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public required string CustomerName { get; set; }
        public required string CustomerEmail { get; set; }
        public required decimal TotalPrice { get; set; }
        public List<OrderItemEvent> Items { get; set; } = new();
    }

    public class OrderItemEvent
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}