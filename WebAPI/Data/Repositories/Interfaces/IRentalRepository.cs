using WebAPI.Data.Models;
using WebAPI.filters;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IRentalRepository
{
    Task<List<Rental>> GetRentalsAsync(RentalFilter? filter);
    Task<Rental?> CreateRentalFromOfferAsync(int offerId);
    Task<Rental?> GetRentalByOfferIdAsync(int offerId);
}