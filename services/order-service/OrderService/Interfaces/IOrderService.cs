using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrder(CreateOrderRequest request, string customerId);
    Task<List<Order>> GetAllOrders();
    Task<List<Order>> GetCustomerAllOrders(string customerId);
    Task<Order> GetOrderById(string customerId, Guid id);
}