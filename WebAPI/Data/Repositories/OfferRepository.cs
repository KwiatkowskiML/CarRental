using Microsoft.EntityFrameworkCore;
using WebAPI.Constants;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;

namespace WebAPI.Data.Repositories;

public class OfferRepository(CarRentalContext context, ILogger logger)
    : BaseRepository<Offer>(context, logger), IOfferRepository
{
    public async Task<Offer?> GetOfferAsync(OfferFilter filter)
    {
        try
        {
            var query = Context.Offers
                .Include(o => o.Customer)
                .ThenInclude(cust => cust!.User)
                .Include(o => o.Car)
                .ThenInclude(c => c!.CarProvider)
                .Include(o => o.Insurance)
                .AsQueryable();

            // Apply filters conditionally
            if (filter.OfferId.HasValue)
                query = query.Where(o => o.OfferId == filter.OfferId);

            if (filter.TotalPrice.HasValue)
                query = query.Where(o => o.TotalPrice == filter.TotalPrice);

            if (filter.CustomerId.HasValue)
                query = query.Where(o => o.CustomerId == filter.CustomerId);

            if (filter.CarId.HasValue)
                query = query.Where(o => o.CarId == filter.CarId);

            if (filter.InsuranceId.HasValue)
                query = query.Where(o => o.InsuranceId == filter.InsuranceId);

            if (filter.StartDate.HasValue)
                query = query.Where(o => o.StartDate == filter.StartDate);

            if (filter.EndDate.HasValue)
                query = query.Where(o => o.EndDate == filter.EndDate);

            if (filter.CreatedAt.HasValue)
                query = query.Where(o => o.CreatedAt == filter.CreatedAt);

            if (filter.HasGps.HasValue)
                query = query.Where(o => o.HasGps == filter.HasGps);

            if (filter.HasChildSeat.HasValue)
                query = query.Where(o => o.HasChildSeat == filter.HasChildSeat);

            var offer = await query.FirstOrDefaultAsync();
            return offer;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching offer with filter");
            throw new DatabaseOperationException("Failed to fetch offer with specified filter", ex);
        }
    }

    public async Task<List<Insurance>> GetInsurancesAsync()
    {
        try
        {
            return await Context.Insurances
                .AsNoTracking()
                .OrderBy(i => i.Price)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching insurances");
            throw new DatabaseOperationException("Failed to fetch insurances", ex);
        }
    }

    public async Task<Insurance?> GetInsuranceByIdAsync(int insuranceId)
    {
        try
        {
            return await Context.Insurances
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InsuranceId == insuranceId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching insurance by id {InsuranceId}", insuranceId);
            throw new DatabaseOperationException($"Failed to fetch insurance with ID {insuranceId}", ex);
        }
    }

    public async Task CreateOfferAsync(Offer offer)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            var car = await Context.Cars
                .FirstOrDefaultAsync(c => c.CarId == offer.CarId);

            if (car == null)
                throw new InvalidOperationException($"Car with ID {offer.CarId} not found");

            if (offer.StartDate > offer.EndDate)
                throw new InvalidOperationException("Invalid date range");

            var hasConflict = await Context.Rentals
                .AnyAsync(r => r.Offer.CarId == offer.CarId &&
                               r.RentalStatusId != RentalStatus.GetCompletedId() &&
                               r.Offer.StartDate <= offer.EndDate &&
                               r.Offer.EndDate >= offer.StartDate);

            if (hasConflict)
                throw new InvalidOperationException("Car is not available for the selected dates");

            var customerExists = await Context.Customers
                .AnyAsync(c => c.CustomerId == offer.CustomerId);

            if (!customerExists)
                throw new InvalidOperationException($"Customer with ID {offer.CustomerId} not found");

            var insuranceExists = await Context.Insurances
                .AnyAsync(i => i.InsuranceId == offer.InsuranceId);

            if (!insuranceExists)
                throw new InvalidOperationException($"Insurance with ID {offer.InsuranceId} not found");

            // Set creation timestamp if not already set
            offer.CreatedAt ??= DateTime.UtcNow;

            // Add the offer to the context
            await Context.Offers.AddAsync(offer);
            await Context.SaveChangesAsync();

            await Context.Entry(offer)
                .Reference(o => o.Insurance)
                .LoadAsync();

            // Commit the transaction
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Logger.LogError(ex, "Error creating offer for car {CarId}", offer.CarId);
            throw new DatabaseOperationException("Failed to create offer", ex);
        }
    }

    public async Task DeleteExpiredOffersAsync()
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-OfferCleanupConstants.UnusedOfferExpirationMinutes);

            var staleOffers = await Context.Offers
                .Where(o => o.CreatedAt <= cutoffTime &&
                            !Context.Rentals.Any(r => r.OfferId == o.OfferId))
                .ToListAsync();

            if (staleOffers.Any())
            {
                Context.Offers.RemoveRange(staleOffers);
                var deletedCount = await Context.SaveChangesAsync();
                Logger.LogInformation(
                    "Deleted {Count} stale offers created before {CutoffTime}",
                    deletedCount,
                    cutoffTime);
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Logger.LogError(ex, "Error deleting stale offers");
            throw new DatabaseOperationException("Failed to delete stale offers", ex);
        }
    }
}