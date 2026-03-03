using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Interfaces;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("orders/{customerId}")]  
        public async Task<IActionResult> GetCustomerOrders(string customerId)
        {
            var orders = await _orderService.GetCustomerAllOrders(customerId);
            return Ok(orders);
        }

        [HttpGet("orders/{customerId}/{orderId}")]
        public async Task<IActionResult> GetOrderById(string customerId, Guid orderId)
        {
            var order = await _orderService.GetOrderById(customerId, orderId);
            return Ok(order);
        }
    }
}