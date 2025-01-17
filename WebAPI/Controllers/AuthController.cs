using Microsoft.AspNetCore.Mvc;
using WebAPI.Auth;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Requests;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GoogleAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        GoogleAuthService authService,
        IUnitOfWork unitOfWork,
        ILogger<AuthController> logger)
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
                BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-request.Age)),
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