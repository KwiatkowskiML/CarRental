using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.Data.Repositories;

public class RentalRepository(CarRentalContext context, ILogger logger) : BaseRepository<Rental>(context, logger), IRentalRepository
{
    public async Task<List<RentalDTO>> GetUserRentalsAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<RentalDTO?> CreateRentalFromOfferAsync(int offerId)
    {
        throw new NotImplementedException();
    }

    public async Task<RentalDTO?> GetRentalByOfferIdAsync(int offerId)
    {
        throw new NotImplementedException();
    }
}