using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Maps;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.filters;

namespace WebAPI.Data.Repositories;

public class OfferRepository(CarRentalContext context, ILogger<OfferRepository> logger) : BaseRepository<Offer>(context, logger), IOfferRepository
{
    public async Task<OfferDTO?> GetOffersAsync(OfferFilter filter)
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
}