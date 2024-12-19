using WebAPI.Data.Models;
using WebAPI.filters;
using WebAPI.Requests;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IRentalRepository
{
    Task<List<Rental>> GetRentalsAsync(RentalFilter? filter);
    Task<Rental?> CreateRentalFromOfferAsync(int offerId);
    Task<Rental?> GetRentalByOfferIdAsync(int offerId);
    Task<bool> InitReturn(int rentalId);
    Task<bool> ProcessReturn(AcceptReturnRequest request);
}