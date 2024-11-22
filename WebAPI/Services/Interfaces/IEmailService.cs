namespace CarRental.WebAPI.Services.Interfaces;

public interface IEmailService
{
    Task SendRentalConfirmationEmail(string toEmail, string userName, string confirmationLink);
}