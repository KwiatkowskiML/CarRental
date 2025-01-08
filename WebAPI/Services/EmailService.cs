using CarRental.WebAPI.Services.Options;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services;

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

This link will expire in 10 minutes and can only be used with your account.

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
                <li>This link will expire in 10 minutes</li>
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

    public async Task SendRentalSuccessEmail(string toEmail, string userName, string rentalManagementLink)
    {
        try
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.FromEmail, _options.FromName),
                Subject = "Your Car Rental is Confirmed!",
                PlainTextContent = $@"
    Hello {userName},

    Great news! Your car rental has been successfully confirmed. 

    You can manage your rental details, including viewing pickup instructions, modifying your reservation, or canceling if needed, through our rental management portal:

    {rentalManagementLink}

    If you have any questions or need assistance, please don't hesitate to contact our support team.

    Thank you for choosing our car rental service!

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
            .success-icon {{
                font-size: 48px;
                color: #28a745;
                text-align: center;
                margin: 20px 0;
            }}
        </style>
    </head>
    <body>
        <div class='header'>
            <h1>Rental Successfully Confirmed!</h1>
        </div>
        <div class='container'>
            <div class='success-icon'>✓</div>
            <h2>Hello {userName},</h2>
            <p>Great news! Your car rental has been successfully confirmed.</p>
            
            <p>You can now access our rental management portal to:</p>
            <ul>
                <li>View your rental details</li>
                <li>Access pickup instructions</li>
                <li>Modify your reservation</li>
                <li>Cancel if needed</li>
            </ul>

            <p style='text-align: center;'>
                <a href='{rentalManagementLink}' class='button'>Manage Your Rental</a>
            </p>
            
            <div class='note'>
                <p><strong>Need to access later?</strong></p>
                <p>Bookmark this link to manage your rental:</p>
                <p style='word-break: break-all;'><small>{rentalManagementLink}</small></p>
            </div>

            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
            
            <div class='footer'>
                <p>Thank you for choosing our car rental service!</p>
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
            _logger.LogError(ex, "Error sending rental success email to {Email}", toEmail);
            throw;
        }
    }

    public async Task SendReturnProcessInitiatedEmail(string toEmail, string userName)
    {
        try
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.FromEmail, _options.FromName),
                Subject = "Your Car Rental Return Process Has Started",
                PlainTextContent = $@"
    Hello {userName},

    We have initiated the return process for your car rental. 

    If you have any questions or need further assistance, feel free to reach out to our support team.

    Thank you for using our car rental service.

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
            <h1>Return Process Started</h1>
        </div>
        <div class='container'>
            <h2>Hello {userName},</h2>
            <p>We have initiated the return process for your car rental.</p>
            <p>If you have any questions or need further assistance, feel free to reach out to our support team.</p>
            <div class='footer'>
                <p>Thank you for using our car rental service!</p>
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
            _logger.LogError(ex, "Error sending return process initiated email to {Email}", toEmail);
            throw;
        }
    }
}