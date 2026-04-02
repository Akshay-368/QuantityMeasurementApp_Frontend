using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using QuantityMeasurement.ModelLayer.DTOs;

namespace QuantityMeasurement.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[EnableRateLimiting("fixedWindowLimiter")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRequestDto request)
    {
        await _authService.RegisterAsync(request.Username, request.Password);
        return Ok("User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequestDto request)
    {
        var token = await _authService.LoginAsync(request.Username, request.Password);
        return Ok(token);
    }
}

