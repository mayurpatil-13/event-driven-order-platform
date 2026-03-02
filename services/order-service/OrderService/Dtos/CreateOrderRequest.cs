namespace OrderService.Dtos;

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> Items { get; set; } = new List<CreateOrderItemRequest>();
}

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; } = Guid.Empty;
    public int Quantity { get; set; }
}
