using WebAPI.Data.Models;
using WebAPI.filters;
using WebAPI.Requests;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IRentalRepository
{
    Task<List<Rental>> GetRentalsAsync(RentalFilter? filter);
    Task<(List<Rental> Rentals, int TotalCount)> GetPaginatedRentalsAsync(RentalFilter? filter, int page, int pageSize);

    Task<Rental?> CreateRentalFromOfferAsync(int offerId);
    Task<Rental?> GetRentalByOfferIdAsync(int offerId);
    Task<bool> InitReturn(int rentalId);
    Task<Return> ProcessReturn(AcceptReturnRequest request);
}