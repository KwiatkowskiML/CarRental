using System.Security.Cryptography;
using System.Text;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

public class RentalConfirmationService : IRentalConfirmationService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RentalConfirmationService> _logger;

    public RentalConfirmationService(
        IConfiguration configuration,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<RentalConfirmationService> logger)
    {
        _configuration = configuration;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

   public async Task SendConfirmationEmail(int offerId, int customerId, string userEmail, string userName)
    {
        try 
        {
            _logger.LogInformation("Generating token for offerId: {OfferId}, customerId: {CustomerId}", offerId, customerId);
            var token = GenerateConfirmationToken(offerId, customerId);
            _logger.LogInformation("Generated raw token: {Token}", token);

            var frontendUrl = _configuration["FRONTEND_URL"] ?? "http://localhost:5173";
            var encodedToken = Uri.EscapeDataString(token);
            _logger.LogInformation("Encoded token: {EncodedToken}", encodedToken);

            var confirmationLink = $"{frontendUrl}/rental-confirm?token={encodedToken}";
            _logger.LogInformation("Generated confirmation link: {Link}", confirmationLink);

            await _emailService.SendRentalConfirmationEmail(userEmail, userName, confirmationLink);
            _logger.LogInformation("Confirmation email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendConfirmationEmail");
            throw;
        }
    }

    private string GenerateConfirmationToken(int offerId, int userId)
    {
        try 
        {
            var timestamp = DateTime.UtcNow.AddMinutes(10).ToString("u");
            var tokenData = $"{offerId}_{userId}_{timestamp}";
            _logger.LogInformation("Token data before hashing: {TokenData}", tokenData);

            var key = _configuration["JWT_SECRET"];
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenData));
                var hashString = Convert.ToBase64String(hash);
                var finalToken = $"{tokenData}_{hashString}";
                _logger.LogInformation("Generated token with hash: {Token}", finalToken);
                return finalToken;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token");
            throw;
        }
    }

    public (bool isValid, int offerId, int customerId) ValidateConfirmationToken(string token)
    {
        try
        {
            _logger.LogInformation("Starting token validation for token: {Token}", token);

            // First, try to decode the token if it's URL encoded
            var decodedToken = Uri.UnescapeDataString(token);
            _logger.LogInformation("Decoded token: {DecodedToken}", decodedToken);

            var parts = decodedToken.Split('_');
            _logger.LogInformation("Token parts count: {Count}", parts.Length);

            if (parts.Length != 4)
            {
                _logger.LogWarning("Invalid token format - expected 4 parts, got {Count}", parts.Length);
                return (false, 0, 0);
            }

            // Try parsing offerId and userId
            if (!int.TryParse(parts[0], out int offerId))
            {
                _logger.LogWarning("Failed to parse offerId: {OfferId}", parts[0]);
                return (false, 0, 0);
            }

            if (!int.TryParse(parts[1], out int customerId))
            {
                _logger.LogWarning("Failed to parse customerId: {CustomerId}", parts[1]);
                return (false, 0, 0);
            }

            // Parse and validate timestamp
            if (!DateTime.TryParseExact(parts[2], "u", null, System.Globalization.DateTimeStyles.None, out DateTime expirationDate))
            {
                _logger.LogWarning("Failed to parse expiration date: {ExpirationDate}", parts[2]);
                return (false, 0, 0);
            }

            _logger.LogInformation("Parsed values - OfferId: {OfferId}, CustomerId: {CustomerId}, ExpirationDate: {ExpirationDate}", 
                offerId, customerId, expirationDate);

            if (expirationDate < DateTime.UtcNow)
            {
                _logger.LogWarning("Token expired. Expiration: {Expiration}, Current: {Current}", 
                    expirationDate, DateTime.UtcNow);
                return (false, 0, 0);
            }

            // Validate hash
            var receivedHash = parts[3];
            var tokenData = $"{offerId}_{customerId}_{expirationDate:u}";
            var key = _configuration["JWT_SECRET"];

            _logger.LogInformation("Recreating hash for validation. TokenData: {TokenData}", tokenData);

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var computedHash = Convert.ToBase64String(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(tokenData)));
                
                _logger.LogInformation("Hash comparison - Received: {ReceivedHash}, Computed: {ComputedHash}", 
                    receivedHash, computedHash);

                if (receivedHash != computedHash)
                {
                    _logger.LogWarning("Hash mismatch");
                    return (false, 0, 0);
                }
            }

            _logger.LogInformation("Token validation successful");
            return (true, offerId, customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return (false, 0, 0);
        }
    }
}