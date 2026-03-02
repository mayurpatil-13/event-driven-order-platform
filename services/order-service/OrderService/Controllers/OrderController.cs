using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos;
using OrderService.Interfaces;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("orders")]
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
                var order = await _orderService.CreateOrder(request);
                return CreatedAtAction(nameof(CreateOrder), new { id = order.Id }, order);
              }
              catch (Exception ex)
              {
                return BadRequest(new { error = ex.Message });
              }
         }
    
          [HttpGet("{id}")]
          public IActionResult GetOrder(Guid id)
          {
                // This is a placeholder for the GetOrder endpoint.
                // You can implement this method to retrieve an order by its ID.
                return Ok(new { message = $"GetOrder endpoint called with ID: {id}" });
       }
    }
}