using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Maps;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.filters;

namespace WebAPI.Data.Repositories;

public class OfferRepository(CarRentalContext context, ILogger logger) : BaseRepository<Offer>(context, logger), IOfferRepository
{
    public async Task<OfferDTO?> GetOfferAsync(OfferFilter filter)
    {
        try
        {
            Logger.LogInformation($"Searching for offer - OfferId: {filter.OfferId}, CustomerId: {filter.CustomerId}");

            var query = Context.Offers
                .Include(o => o.Customer)
                .Include(o => o.Car)
                    .ThenInclude(c => c.CarProvider)
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

            // Log the generated SQL query for debugging
            var sql = query.ToQueryString();
            Logger.LogInformation($"Generated SQL: {sql}");

            // Get the first matching offer or null if none found
            var offer = await query.FirstOrDefaultAsync();
            
            // Return null if no offer found
            if (offer == null)
                return null;

            // Map the offer to DTO and return
            return Mapper.OfferToDTO(offer);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching offer with filter");
            throw new DatabaseOperationException("Failed to fetch offer with specified filter", ex);
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

                var hasConflict = await Context.Rentals
                    .AnyAsync(r => r.Offer.CarId == offer.CarId &&
                                r.Status != "cancelled" &&
                                ((r.Offer.StartDate <= offer.EndDate && r.Offer.EndDate >= offer.StartDate) ||
                                (r.Offer.StartDate >= offer.StartDate && r.Offer.StartDate <= offer.EndDate)));

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
}