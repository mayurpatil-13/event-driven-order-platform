namespace InventoryService.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}