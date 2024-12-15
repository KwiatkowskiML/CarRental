using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.DTOs;
using WebAPI.filters;

namespace WebAPI.Data.Repositories;

public class CarRepository(CarRentalContext context, ILogger logger) : BaseRepository<Car>(context, logger), ICarRepository
{
    public async Task<List<Car>> GetCarsAsync(CarFilter filter)
    {
        try
        {
            IQueryable<Car> query = Context.Cars
                .Include(c => c.CarProvider)
                .Where(c => c.Status == "available");
            
            if (filter.CarId.HasValue)
                query = query.Where(c => c.CarId == filter.CarId.Value);

            if (!string.IsNullOrEmpty(filter.Location))
                query = query.Where(c => c.Location == filter.Location);

            if (!string.IsNullOrEmpty(filter.Brand))
                query = query.Where(c => c.Brand.Contains(filter.Brand));

            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(c => c.Model.Contains(filter.Model));

            if (filter.MinYear.HasValue)
                query = query.Where(c => c.Year >= filter.MinYear.Value);

            if (filter.MaxYear.HasValue)
                query = query.Where(c => c.Year <= filter.MaxYear.Value);

            if (!string.IsNullOrEmpty(filter.FuelType))
                query = query.Where(c => c.FuelType == filter.FuelType);

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                var startDate = DateOnly.FromDateTime(filter.StartDate.Value);
                var endDate = DateOnly.FromDateTime(filter.EndDate.Value);
                    
                query = query.Where(c => !c.Offers.Any(o => 
                    o.Rental != null && 
                    o.Rental.Status != "cancelled" &&
                    o.StartDate <= endDate && o.EndDate >= startDate
                ));
            }

            return await query.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching available cars with filters");
            throw new DatabaseOperationException("Failed to fetch available cars", ex);
        }
    }
    
    public async Task<Car?> GetCarByIdAsync(int carId)
    {
        try
        {
            return await Context.Cars
                .Include(c => c.CarProvider)
                .FirstOrDefaultAsync(c => c.CarId == carId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching car by id");
            throw new DatabaseOperationException($"Failed to fetch car with ID {carId}", ex);
        }
    }
}