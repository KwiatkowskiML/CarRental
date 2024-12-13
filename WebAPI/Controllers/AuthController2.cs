using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Auth;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using WebAPI.Data.Repositories.Interfaces;

namespace CarRental.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController2 : ControllerBase
{
    private readonly GoogleAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthController2> _logger;

    public AuthController2(
        GoogleAuthService authService,
        IUnitOfWork unitOfWork,
        ILogger<AuthController2> logger)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var authResult = await _authService.ValidateGoogleTokenAndGenerateJwt(request.Token);
            
            if (authResult.NeedsRegistration)
            {
                return NotFound(new { needsRegistration = true, userData = authResult.UserData });
            }

            return Ok(new { token = authResult.Token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication");
            return BadRequest("Invalid token");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
    {
        try
        {
            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                CreatedAt = DateTime.UtcNow
            };

            user = await _unitOfWork.UsersRepository.CreateUserAsync(user);

            var customer = new Customer
            {
                UserId = user.UserId,
                DrivingLicenseYears = request.DrivingLicenseYears
            };

            await _unitOfWork.UsersRepository.CreateCustomerAsync(customer);

            // Generate JWT token for the new user
            var authResult = await _authService.ValidateGoogleTokenAndGenerateJwt(request.GoogleToken);
            return Ok(new { token = authResult.Token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return BadRequest("Registration failed");
        }
    }
}

public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string GoogleToken { get; set; } = string.Empty;
    public int Age { get; set; }
    public int DrivingLicenseYears { get; set; }
}

public class GoogleLoginRequest
{
    public string Token { get; set; } = string.Empty;
}