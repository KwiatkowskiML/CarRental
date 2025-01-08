namespace WebAPI.Services.Interfaces;

public interface IRentalConfirmationService
{
    Task SendConfirmationEmail(int offerId, int customerId, string userEmail, string userName);
    (bool isValid, int offerId, int customerId) ValidateConfirmationToken(string token);
}