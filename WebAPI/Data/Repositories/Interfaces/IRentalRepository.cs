using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IRentalRepository
{
    Task<List<RentalDTO>> GetUserRentalsAsync(int userId);
    Task<RentalDTO?> CreateRentalFromOfferAsync(int offerId);
    Task<RentalDTO?> GetRentalByOfferIdAsync(int offerId);
}