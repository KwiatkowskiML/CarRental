namespace WebAPI.Services.Interfaces;

public interface IEmailService
{
    Task SendRentalConfirmationEmail(string toEmail, string userName, string confirmationLink);
    Task SendRentalSuccessEmail(string toEmail, string userName, string rentalManagmentLink);
}