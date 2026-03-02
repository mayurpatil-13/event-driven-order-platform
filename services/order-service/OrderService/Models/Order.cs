public enum OrderStatus
{
    Pending,
    confirmed,
    Shipped,
    Delivered,
    Cancelled
}

namespace OrderService.Models
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public string Location { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
