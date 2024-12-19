using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;

namespace WebAPI.Data.Repositories;

public class RentalRepository(CarRentalContext context, ILogger logger)
    : BaseRepository<Rental>(context, logger), IRentalRepository
{
    public async Task<List<Rental>> GetRentalsAsync(RentalFilter? filter)
    {
        try
        {
            var query = Context.Rentals
                .Include(r => r.Offer)
                .ThenInclude(o => o.Customer)
                .ThenInclude(cust => cust!.User)
                .Include(r => r.Offer)
                .ThenInclude(o => o.Car)
                .ThenInclude(c => c!.CarProvider)
                .AsQueryable();
                
            if (filter != null && filter.CustomerId.HasValue)
                query = query.Where(r => r.Offer.Customer != null && r.Offer.Customer.CustomerId == filter.CustomerId);
            
            query = query.OrderByDescending(r => r.CreatedAt);

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching rentals");
            throw new DatabaseOperationException($"Failed to fetch rentals", ex);
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

            if (offer.Car!.Status != "available")
                throw new InvalidOperationException("Car is not available for rental");

            var hasOverlap = await Context.Rentals
                .AnyAsync(r => r.Offer.CarId == offer.CarId &&
                               r.RentalStatusId != RentalStatus.GetCompletedId() &&
                               ((r.Offer.StartDate <= offer.EndDate && r.Offer.EndDate >= offer.StartDate) ||
                                (r.Offer.StartDate >= offer.StartDate && r.Offer.StartDate <= offer.EndDate)));

            if (hasOverlap)
                throw new InvalidOperationException("Car is already booked for these dates");

            // Create new rental
            var rental = new Rental
            {
                OfferId = offerId,
                RentalStatusId = RentalStatus.GetConfirmedId(),
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
                .ThenInclude(c => c!.CarProvider)
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

    public async Task<Rental?> GetRentalByOfferIdAsync(int offerId)
    {
        try
        {
            var rental = await Context.Rentals
                .Include(r => r.Offer)
                .ThenInclude(o => o.Car)
                .ThenInclude(c => c!.CarProvider)
                .Include(r => r.Offer)
                .ThenInclude(o => o.Customer)
                .Include(r => r.Offer)
                .ThenInclude(o => o.Insurance)
                .FirstOrDefaultAsync(r => r.OfferId == offerId);
            
            return rental;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting rental by offerId {OfferId}", offerId);
            throw new DatabaseOperationException($"Failed to get rental for offer {offerId}", ex);
        }
    }
}