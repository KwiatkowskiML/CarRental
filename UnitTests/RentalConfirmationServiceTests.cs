using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Services;
using WebAPI.Services.Interfaces;

public class RentalConfirmationServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RentalConfirmationService>> _loggerMock;
    private readonly string _jwtSecret = "your-test-secret-key-that-is-long-enough-for-hmac";
    private readonly RentalConfirmationService _service;

    public RentalConfirmationServiceTests()
    {
        _configMock = new Mock<IConfiguration>();
        _emailServiceMock = new Mock<IEmailService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RentalConfirmationService>>();

        _configMock.Setup(c => c["JWT_SECRET"]).Returns(_jwtSecret);
        _configMock.Setup(c => c["FRONTEND_URL"]).Returns("http://localhost:5173");

        _service = new RentalConfirmationService(
            _configMock.Object,
            _emailServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Should_Generate_Valid_Confirmation_Token()
    {
        // Arrange
        int offerId = 123;
        int customerId = 456;
        string userEmail = "test@example.com";
        string userName = "Test User";

        // Act
        await _service.SendConfirmationEmail(offerId, customerId, userEmail, userName);
        
        // Verify email was sent with a valid confirmation link
        _emailServiceMock.Verify(e => e.SendRentalConfirmationEmail(
            It.Is<string>(email => email == userEmail),
            It.Is<string>(name => name == userName),
            It.Is<string>(link => link.Contains("/rental-confirm?token="))),
            Times.Once);
    }

    [Fact]
    public void Should_Reject_Expired_Tokens()
    {
        // Arrange
        int offerId = 123;
        int customerId = 456;
        var expiredTimestamp = DateTime.UtcNow.AddMinutes(-11).ToString("u"); // 11 minutes ago
        var tokenData = $"{offerId}_{customerId}_{expiredTimestamp}";
        
        using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_jwtSecret)))
        {
            var hash = Convert.ToBase64String(
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(tokenData)));
            var token = $"{tokenData}_{hash}";

            // Act
            var result = _service.ValidateConfirmationToken(token);

            // Assert
            Assert.False(result.isValid);
            Assert.Equal(0, result.offerId);
            Assert.Equal(0, result.customerId);
        }
    }

    [Fact]
    public void Should_Reject_Tokens_With_Invalid_Signatures()
    {
        // Arrange
        int offerId = 123;
        int customerId = 456;
        var timestamp = DateTime.UtcNow.AddMinutes(5).ToString("u");
        var invalidHash = "invalid_hash_value";
        var token = $"{offerId}_{customerId}_{timestamp}_{invalidHash}";

        // Act
        var result = _service.ValidateConfirmationToken(token);

        // Assert
        Assert.False(result.isValid);
        Assert.Equal(0, result.offerId);
        Assert.Equal(0, result.customerId);
    }

    [Fact]
    public void Should_Reject_Malformed_Tokens()
    {
        // Arrange
        var malformedTokens = new[]
        {
            "invalid_token",
            "123_456_",
            "123_456_timestamp",
            "", // empty token
            null // null token
        };

        // Act & Assert
        foreach (var token in malformedTokens)
        {
            var result = _service.ValidateConfirmationToken(token);
            Assert.False(result.isValid);
            Assert.Equal(0, result.offerId);
            Assert.Equal(0, result.customerId);
        }
    }

    [Fact]
    public void Should_Validate_Token_Components_Correctly()
    {
        // Arrange
        int offerId = 123;
        int customerId = 456;
        var timestamp = DateTime.UtcNow.AddMinutes(5).ToString("u");
        var tokenData = $"{offerId}_{customerId}_{timestamp}";
        
        using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_jwtSecret)))
        {
            var hash = Convert.ToBase64String(
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(tokenData)));
            var token = $"{tokenData}_{hash}";

            // Act
            var result = _service.ValidateConfirmationToken(token);

            // Assert
            Assert.True(result.isValid);
            Assert.Equal(offerId, result.offerId);
            Assert.Equal(customerId, result.customerId);
        }
    }

    [Fact]
    public void Should_Handle_URL_Encoded_Tokens_Correctly()
    {
        // Arrange
        int offerId = 123;
        int customerId = 456;
        var timestamp = DateTime.UtcNow.AddMinutes(5).ToString("u");
        var tokenData = $"{offerId}_{customerId}_{timestamp}";
        
        using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_jwtSecret)))
        {
            var hash = Convert.ToBase64String(
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(tokenData)));
            var token = $"{tokenData}_{hash}";
            var encodedToken = Uri.EscapeDataString(token);

            // Act
            var result = _service.ValidateConfirmationToken(encodedToken);

            // Assert
            Assert.True(result.isValid);
            Assert.Equal(offerId, result.offerId);
            Assert.Equal(customerId, result.customerId);
        }
    }

    [Fact]
    public void Should_Reject_Tokens_With_Mismatched_User_IDs()
    {
        // Arrange
        int offerId = 123;
        int correctCustomerId = 456;
        int wrongCustomerId = 789;
        var timestamp = DateTime.UtcNow.AddMinutes(5).ToString("u");
        
        // Generate token with correct customer ID
        var correctTokenData = $"{offerId}_{correctCustomerId}_{timestamp}";
        using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_jwtSecret)))
        {
            var hash = Convert.ToBase64String(
                hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(correctTokenData)));
            
            // Create token with wrong customer ID but correct hash
            var tokenWithWrongId = $"{offerId}_{wrongCustomerId}_{timestamp}_{hash}";

            // Act
            var result = _service.ValidateConfirmationToken(tokenWithWrongId);

            // Assert
            Assert.False(result.isValid);
            Assert.Equal(0, result.offerId);
            Assert.Equal(0, result.customerId);
        }
    }
}