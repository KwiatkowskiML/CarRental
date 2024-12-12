using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.Data.Repositories;

public class RentalRepository(CarRentalContext context, ILogger logger) : BaseRepository<Rental>(context, logger), IRentalRepository
{
    public async Task<List<Rental>> GetUserRentalsAsync(int userId)
    {
        try
        {
            var query = Context.Rentals
                .Include(r => r.Offer)
                .ThenInclude(o => o.Customer)
                .Include(r => r.Offer)
                .ThenInclude(o => o.Car)
                .ThenInclude(c => c.CarProvider)
                .Where(r => r.Offer.Customer != null && r.Offer.Customer.UserId == userId)
                .OrderByDescending(r => r.CreatedAt);
            
            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching user rentals");
            throw new DatabaseOperationException($"Failed to fetch rentals for user {userId}", ex);
        }
    }

    public async Task<Rental?> CreateRentalFromOfferAsync(int offerId)
    {
        throw new NotImplementedException();
    }

    public async Task<Rental?> GetRentalByOfferIdAsync(int offerId)
    {
        throw new NotImplementedException();
    }
}