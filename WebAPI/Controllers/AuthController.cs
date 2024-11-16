using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Auth;

namespace CarRental.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GoogleAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        GoogleAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var jwt = await _authService.ValidateGoogleTokenAndGenerateJwt(request.Token);
            return Ok(new { token = jwt });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication");
            return BadRequest("Invalid token");
        }
    }
}

public class GoogleLoginRequest
{
    public string Token { get; set; } = string.Empty;
}