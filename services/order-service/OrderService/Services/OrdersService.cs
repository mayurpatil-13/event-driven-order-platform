using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using OrderProcessorWorker.Models;
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
                order.OrderStatus = OrderStatus.Pending;
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            await _kafkaProducer.PublishOrderRequestedEvent(new OrderRequestedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
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

        public async Task UpdateOrderStatus(OrderUpdateEvent orderUpdateEvent)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderUpdateEvent.OrderId);
            if (order == null)
                throw new InvalidOperationException("Order not found");
                
            if (!orderUpdateEvent.IsSuccess)
            {
                var failedProducts = new List<Guid>();
                orderUpdateEvent.InventoryResults.ForEach(result =>
                {
                    if (!result.Success)
                    {
                        Console.WriteLine($"Insufficient inventory for ProductId: {result.ProductId}");
                        failedProducts.Add(result.ProductId);
                    }
                });
                order.OrderStatus = OrderStatus.Failed;
                order.Message = "Failed to update order status due to insufficient inventory for products: " + string.Join(", ", failedProducts);
            }
            else
            {
                order.OrderStatus = OrderStatus.Confirmed;
                order.Message = "Order confirmed successfully.";
            }
            order.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
    }
}