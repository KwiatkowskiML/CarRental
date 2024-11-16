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

    public GoogleAuthService(
        IOptions<GoogleAuthOptions> googleOptions,
        IOptions<JwtOptions> jwtOptions,
        ICarRentalRepository repository)
    {
        _googleOptions = googleOptions.Value;
        _jwtOptions = jwtOptions.Value;
        _repository = repository;
    }

    public async Task<string> ValidateGoogleTokenAndGenerateJwt(string googleToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _googleOptions.ClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
        
        // Check if user exists, if not create new user
        var user = await _repository.GetUserByEmail(payload.Email);
        if (user == null)
        {
            user = new User
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.CreateUser(user);
        }

        return GenerateJwt(user);
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