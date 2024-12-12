using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IRentalRepository
{
    Task<List<Rental>> GetUserRentalsAsync(int userId);
    Task<Rental?> CreateRentalFromOfferAsync(int offerId);
    Task<Rental?> GetRentalByOfferIdAsync(int offerId);
}