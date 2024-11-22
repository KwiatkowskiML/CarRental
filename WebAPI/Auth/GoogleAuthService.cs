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

    public async Task<AuthResult> ValidateGoogleTokenAndGenerateJwt(string googleToken)
    {
        try 
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleOptions.ClientId },
                IssuedAtClockTolerance = TimeSpan.FromMinutes(5),
                ExpirationTimeClockTolerance = TimeSpan.FromMinutes(5)
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
            var user = await _repository.GetUserByEmail(payload.Email);

            if (user == null)
            {
                return new AuthResult 
                { 
                    NeedsRegistration = true,
                    UserData = new UserData 
                    {
                        Email = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Token = googleToken
                    }
                };
            }

            return new AuthResult 
            { 
                Token = GenerateJwt(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication failed");
            throw new AuthenticationException("Authentication failed", ex);
        }
    }

    public class AuthResult
    {
        public string? Token { get; set; }
        public bool NeedsRegistration { get; set; }
        public UserData? UserData { get; set; }
    }

    public class UserData
    {
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Token { get; set; } = string.Empty;
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