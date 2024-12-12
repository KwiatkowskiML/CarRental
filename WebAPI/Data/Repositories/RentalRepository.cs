using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Mappers;
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

    // TODO: look into it
    public async Task<Rental?> CreateRentalFromOfferAsync(int offerId)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            var offer = await Context.Offers
                .Include(o => o.Car)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OfferId == offerId);

            if (offer == null)
                return null;

            if (offer.Car.Status != "available")
                throw new InvalidOperationException("Car is not available for rental");

            var hasOverlap = await Context.Rentals
                .AnyAsync(r => r.Offer.CarId == offer.CarId &&
                            r.Status != "cancelled" &&
                            ((r.Offer.StartDate <= offer.EndDate && r.Offer.EndDate >= offer.StartDate) ||
                            (r.Offer.StartDate >= offer.StartDate && r.Offer.StartDate <= offer.EndDate)));

            if (hasOverlap)
                throw new InvalidOperationException("Car is already booked for these dates");

            // Create new rental
            var rental = new Rental
            {
                OfferId = offerId,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };

            // Add rental and update car status
            Context.Rentals.Add(rental);
            
            await Context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Load necessary navigation properties for the DTO
            await Context.Entry(rental)
                .Reference(r => r.Offer)
                .Query()
                .Include(o => o.Car)
                    .ThenInclude(c => c.CarProvider)
                .Include(o => o.Customer)
                .Include(o => o.Insurance)
                .LoadAsync();

            return rental;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Logger.LogError(ex, "Error creating rental for offer {OfferId}", offerId);
            throw new DatabaseOperationException("Failed to create rental", ex);
        }
    }

    // TODO: look into it
    public async Task<Rental?> GetRentalByOfferIdAsync(int offerId)
    {
        throw new NotImplementedException();
    }
}