using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrder(CreateOrderRequest request);
}