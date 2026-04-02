using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.ModelLayer.DTOs;
using QuantityMeasurement.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
namespace QuantityMeasurement.API.Controllers;
/// <summary>
/// This is a controller for the Quantity.
/// To expose the service to the outside world through HTTP
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("fixedWindowLimiter")]
public class QuantityController : ControllerBase
{
    private readonly IQuantityService _quantityService;

    public QuantityController(IQuantityService quantityService)
    {
        _quantityService = quantityService;
    }

    [HttpPost("convert")]
    public async Task<IActionResult> Convert([FromBody] QuantityRequestDto request)
    {
        var result = await _quantityService.ConvertAsync(request);

        return Ok(result);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] QuantityRequestDto request)
    {
        return Ok(await _quantityService.AddAsync(request));
    }

    [HttpPost("subtract")]
    public async Task<IActionResult> Subtract([FromBody] QuantityRequestDto request)
    {
        return Ok(await _quantityService.SubtractAsync(request));
    }

    /// <summary>
    /// Divide the quantity by a scalar.
    /// </summary>
    /// <param name="request">The quantity and scalar to divide by.</param>
    /// <returns>The result of the division.</returns>
    [HttpPost("divide-scalar")]
    public async Task<IActionResult> DivideScalar([FromBody] QuantityRequestDto request)
    {
        return Ok(await _quantityService.DivideByScalarAsync(request));
    }

    [HttpPost("divide-quantity")]
    public async Task<IActionResult> DivideQuantity([FromBody] QuantityRequestDto request)
    {
        return Ok(await _quantityService.DivideByQuantityAsync(request));
    }

}