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

Thank you for choosing our car rental service. Please confirm your rental by clicking the link below:

{confirmationLink}

This link will expire in 24 hours and can only be used with your account.

If you did not request this rental, please ignore this email.

Best regards,
Car Rental Team",
                HtmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            line-height: 1.6; 
            color: #333;
            margin: 0;
            padding: 0;
        }}
        .container {{ 
            max-width: 600px; 
            margin: 0 auto; 
            padding: 20px; 
        }}
        .header {{
            background-color: #8B4513;
            color: white;
            padding: 20px;
            text-align: center;
            margin-bottom: 30px;
        }}
        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #8B4513;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            margin: 20px 0;
        }}
        .note {{
            background-color: #f8f9fa;
            padding: 15px;
            border-radius: 4px;
            margin: 20px 0;
            font-size: 0.9em;
            color: #666;
        }}
        .footer {{ 
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 0.9em;
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Car Rental Confirmation</h1>
    </div>
    <div class='container'>
        <h2>Hello {userName},</h2>
        <p>Thank you for choosing our car rental service. Please confirm your rental by clicking the button below:</p>
        <p style='text-align: center;'>
            <a href='{confirmationLink}' class='button'>Confirm Rental</a>
        </p>
        <div class='note'>
            <p><strong>Important:</strong></p>
            <ul>
                <li>This link will expire in 24 hours</li>
                <li>This link can only be used with your account</li>
                <li>If you cannot click the button, copy and paste this link into your browser:</li>
            </ul>
            <p style='word-break: break-all;'><small>{confirmationLink}</small></p>
        </div>
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
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending rental confirmation email to {Email}", toEmail);
            throw;
        }
    }
}