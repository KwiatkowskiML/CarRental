using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using CarRental.WebAPI.Services.Interfaces;
using CarRental.WebAPI.Services.Options;

namespace CarRental.WebAPI.Services;

public class EmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailOptions> options,
        ILogger<EmailService> logger)
    {
        _options = options.Value;
        _client = new SendGridClient(_options.ApiKey);
        _logger = logger;
    }

    public async Task SendRentalConfirmationEmail(string toEmail, string userName, string confirmationLink)
    {
        try
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.FromEmail, _options.FromName),
                Subject = "Confirm Your Car Rental",
                PlainTextContent = $@"
Hello {userName},

Please confirm your car rental by clicking the following link:
{confirmationLink}

This link will expire in 24 hours.

If you did not request this rental, please ignore this email.

Best regards,
Car Rental Team",
                HtmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #8B4513;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            margin: 20px 0;
        }}
        .footer {{ margin-top: 30px; font-size: 0.9em; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Hello {userName},</h2>
        <p>Please confirm your car rental by clicking the button below:</p>
        <p>
            <a href='{confirmationLink}' class='button'>Confirm Rental</a>
        </p>
        <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
        <p><small>{confirmationLink}</small></p>
        <p>This link will expire in 24 hours.</p>
        <p>If you did not request this rental, please ignore this email.</p>
        <div class='footer'>
            <p>Best regards,<br>Car Rental Team</p>
        </div>
    </div>
</body>
</html>"
            };
            msg.AddTo(new EmailAddress(toEmail));

            var response = await _client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send email. Status code: {StatusCode}", response.StatusCode);
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
            }

            _logger.LogInformation("Successfully sent confirmation email to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending rental confirmation email to {Email}", toEmail);
            throw;
        }
    }
}