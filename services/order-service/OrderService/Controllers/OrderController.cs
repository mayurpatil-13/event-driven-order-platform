using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Interfaces;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

       [HttpPost]
       public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
       {
              try
              {
                Console.WriteLine($"Received CreateOrder request for a: {User.Claims}, b: {User.FindFirst("id")}");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                }
                var customerId = User.FindFirst("id")?.Value ?? string.Empty;
                if (customerId == string.Empty)
                {
                    return Unauthorized(new { error = "Customer ID is missing in token" });
                }
                var order = await _orderService.CreateOrder(request, customerId);
                return CreatedAtAction(nameof(CreateOrder), new { id = order.Id }, order);
              }
              catch (Exception ex)
              {
                return BadRequest(new { error = ex.Message });
              }
         }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
              var customerId = User.FindFirst("id")?.Value ?? string.Empty;
              if (customerId == string.Empty)
              {
                return Unauthorized(new { error = "Customer ID is missing in token" });
              }
              var orders = await _orderService.GetCustomerAllOrders(customerId);
              return Ok(orders);
        }
    
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
              if (id == Guid.Empty)
              {
                return BadRequest(new { error = "Order ID is required" });
              }
              var customerId = User.FindFirst("id")?.Value ?? string.Empty;
              if (customerId == string.Empty)
              {
                return Unauthorized(new { error = "Customer ID is missing in token" });
              }
              var order = await _orderService.GetOrderById(customerId,id);
              return Ok(order);
        }

        [HttpGet("OrderStatus/{id}")]
        public async Task<IActionResult> GetOrderStatus(Guid id)
        {
              if (id == Guid.Empty)
              {
                return BadRequest(new { error = "Order ID is required" });
              }
              var customerId = User.FindFirst("id")?.Value ?? string.Empty;
              if (customerId == string.Empty)
              {
                return Unauthorized(new { error = "Customer ID is missing in token" });
              }
              var order = await _orderService.GetOrderById(customerId,id);
              return Ok(new { order, order.Message });
        }

    }
}