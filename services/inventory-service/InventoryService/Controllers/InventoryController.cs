using InventoryService.Interfaces;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }


    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveInventory([FromBody] ReserveInventoryRequest request)
    {
        var result = await _inventoryService.ReserveStockAsync(request);
        if(result == null || result.Count == 0)
        {
            return BadRequest("No items to reserve");
        }
        return Ok(result);
    }
}