using Microsoft.AspNetCore.Mvc;

namespace api_gateway.controllers
{

    [ApiController]
    [Route("health")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult Check()
        {
            return Ok(new { status = "API Gateway is running." });
        }
    }
}