using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.ModelLayer.DTOs;
using QuantityMeasurement.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
namespace QuantityMeasurement.API.Controllers;

[ApiController] // defines behaviour
[Route("api/[controller]")] // defines endpoint
[EnableRateLimiting("fixedWindowLimiter")]
[Authorize] // defines access rule  ( now everything here will require authentication) 
// Think of them in the order like : This is a controller -> at this route -> and it requires auth.
// and order does not matter , scope does matter .
// class level means applies to all of the endpoints and method level means overrides class
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet]
    public ActionResult<List<HistoryDto>> Get()
    {
        var history = _historyService.GetHistory();
        return Ok(history);
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        _historyService.ClearHistory();
        return NoContent();
    }
}