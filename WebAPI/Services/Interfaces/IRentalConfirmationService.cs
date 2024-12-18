namespace WebAPI.Services.Interfaces;

public interface IRentalConfirmationService
{
    Task SendConfirmationEmail(int offerId, int userId, string userEmail, string userName);
    (bool isValid, int offerId, int userId) ValidateConfirmationToken(string token);
}