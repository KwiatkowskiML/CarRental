using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Auth;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.Repositories.Interfaces;

namespace CarRental.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GoogleAuthService _authService;
    private readonly ICarRentalRepository _repository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        GoogleAuthService authService,
        ICarRentalRepository repository,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _repository = repository;
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

            user = await _repository.CreateUser(user);

            var customer = new Customer
            {
                UserId = user.UserId,
                DrivingLicenseYears = request.DrivingLicenseYears
            };

            await _repository.CreateCustomer(customer);

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