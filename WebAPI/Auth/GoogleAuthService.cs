using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.Repositories.Interfaces;

namespace CarRental.WebAPI.Auth;

public class GoogleAuthService
{
    private readonly GoogleAuthOptions _googleOptions;
    private readonly JwtOptions _jwtOptions;
    private readonly ICarRentalRepository _repository;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(
        IOptions<GoogleAuthOptions> googleOptions,
        IOptions<JwtOptions> jwtOptions,
        ICarRentalRepository repository,
        ILogger<GoogleAuthService> logger)
    {
        _googleOptions = googleOptions.Value;
        _jwtOptions = jwtOptions.Value;
        _repository = repository;
        _logger = logger;
    }

    public async Task<string> ValidateGoogleTokenAndGenerateJwt(string googleToken)
    {
        try 
        {
            _logger.LogInformation("Validating Google token with Client ID: {ClientId}", _googleOptions.ClientId);
            
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleOptions.ClientId },
                IssuedAtClockTolerance = TimeSpan.FromMinutes(5),
                ExpirationTimeClockTolerance = TimeSpan.FromMinutes(5)
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
            _logger.LogInformation("Successfully validated Google token for email: {Email}", payload.Email);

            // Check if user exists, if not create new user
            var user = await _repository.GetUserByEmail(payload.Email);
            if (user == null)
            {
                _logger.LogInformation("Creating new user for email: {Email}", payload.Email);
                user = new User
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? "Unknown",
                    LastName = payload.FamilyName ?? "Unknown",
                    Age = 0, // Default value
                    CreatedAt = DateTime.UtcNow.ToUniversalTime() // Ensure UTC
                };
                user = await _repository.CreateUser(user);

                // Create corresponding customer record with default values
                var customer = new Customer
                {
                    UserId = user.UserId,
                    DrivingLicenseYears = 2 // Default value
                };
                await _repository.CreateCustomer(customer);
            }

            return GenerateJwt(user);
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogError(ex, "Invalid JWT token. ClientID: {ClientId}", _googleOptions.ClientId);
            throw new AuthenticationException("Failed to validate Google token", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Google token validation");
            throw new AuthenticationException("Authentication failed", ex);
        }
    }

    private string GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message)
    {
    }

    public AuthenticationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}