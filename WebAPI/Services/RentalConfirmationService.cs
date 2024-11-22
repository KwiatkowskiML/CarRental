using System;
using System.Security.Cryptography;
using System.Text;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CarRental.WebAPI.Services;

public class RentalConfirmationService : IRentalConfirmationService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ICarRentalRepository _repository;
    private readonly ILogger<RentalConfirmationService> _logger;

    public RentalConfirmationService(
        IConfiguration configuration,
        IEmailService emailService,
        ICarRentalRepository repository,
        ILogger<RentalConfirmationService> logger)
    {
        _configuration = configuration;
        _emailService = emailService;
        _repository = repository;
        _logger = logger;
    }

    public async Task SendConfirmationEmail(int offerId, int userId, string userEmail, string userName)
    {
        // Generate a secure token that includes offerId and userId
        var token = GenerateConfirmationToken(offerId, userId);
        
        // Create the confirmation link
        var baseUrl = _configuration["AppSettings:BaseUrl"];
        var confirmationLink = $"{baseUrl}/api/rentals/confirm?token={token}";

        // Send the email
        await _emailService.SendRentalConfirmationEmail(userEmail, userName, confirmationLink);
    }

    private string GenerateConfirmationToken(int offerId, int userId)
    {
        var tokenData = $"{offerId}:{userId}:{DateTime.UtcNow.AddHours(24):u}";
        var key = _configuration["Jwt:Secret"];
        
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenData));
            return $"{tokenData}:{Convert.ToBase64String(hash)}";
        }
    }

    public (bool isValid, int offerId, int userId) ValidateConfirmationToken(string token)
    {
        try
        {
            var parts = token.Split(':');
            if (parts.Length != 4)
                return (false, 0, 0);

            var offerId = int.Parse(parts[0]);
            var userId = int.Parse(parts[1]);
            var expirationDate = DateTime.ParseExact(parts[2], "u", null);
            var hash = parts[3];

            if (expirationDate < DateTime.UtcNow)
                return (false, 0, 0);

            var tokenData = $"{offerId}:{userId}:{expirationDate:u}";
            var key = _configuration["Jwt:Secret"];

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var computedHash = Convert.ToBase64String(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenData)));
                
                if (hash != computedHash)
                    return (false, 0, 0);
            }

            return (true, offerId, userId);
        }
        catch
        {
            return (false, 0, 0);
        }
    }
}