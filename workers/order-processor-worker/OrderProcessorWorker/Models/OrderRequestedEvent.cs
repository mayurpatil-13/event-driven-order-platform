namespace OrderProcessorWorker.Models;

public class OrderRequestedEvent
{
    public Guid OrderId { get; set; }
    public required string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}