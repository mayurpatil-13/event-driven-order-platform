using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Dtos;
using OrderService.Interfaces;
using OrderService.Kafka;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _dbContext;
        private readonly IInventoryClient _inventoryClient;

        private readonly KafkaProducer _kafkaProducer;

        public OrderService(OrderDbContext dbContext, IInventoryClient inventoryClient, KafkaProducer kafkaProducer)
        {
            _dbContext = dbContext;
            _inventoryClient = inventoryClient;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<Order> CreateOrder(CreateOrderRequest request, string customerId)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderDate = DateTime.UtcNow,
                Location = request.Location,
                CustomerId = customerId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                OrderStatus = OrderStatus.Pending
            };

            foreach (var item in request.Items)
            {
                var product = await _inventoryClient.GetProduct(item.ProductId)
                 ?? throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductName = product.ProductName,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);
                order.TotalPrice += orderItem.Quantity * orderItem.UnitPrice;
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            await _kafkaProducer.PublishOrderCreatedEvent(new OrderCreatedEvent
            {
                OrderId = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                TotalPrice = order.TotalPrice,
                Items = [.. order.OrderItems.Select(item => new OrderItemEvent
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                })]
            });

            return order;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _dbContext.Orders.ToListAsync();
        }

        public async Task<List<Order>> GetCustomerAllOrders(string customerId)
        {
            var orders = _dbContext.Orders.AsQueryable();
            orders = orders.Where(o => o.CustomerId == customerId);

            return await orders.ToListAsync();
        }

        public async Task<Order> GetOrderById(string customerId, Guid id)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);
            if (order == null)
                throw new InvalidOperationException("Order not found");

            return order;
        }
    }
}