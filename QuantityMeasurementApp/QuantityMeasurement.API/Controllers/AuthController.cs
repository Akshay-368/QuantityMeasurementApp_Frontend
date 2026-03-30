using Microsoft.AspNetCore.Mvc;
using QuantityMeasurement.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;


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
    public IActionResult Register(string username, string password)
    {
        _authService.Register(username, password);
        return Ok("User created");
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        var token = _authService.Login(username, password);
        return Ok(token);
    }
}

