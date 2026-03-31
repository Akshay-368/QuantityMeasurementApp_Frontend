using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuantityMeasurement.HistoryService.DTOs;
using QuantityMeasurement.HistoryService.Interfaces;

namespace QuantityMeasurement.HistoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixedWindowLimiter")]
[Authorize]
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
