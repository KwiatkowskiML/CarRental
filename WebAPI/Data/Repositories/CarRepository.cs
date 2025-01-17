using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;

namespace WebAPI.Data.Repositories;

public class CarRepository(CarRentalContext context, ILogger logger) : BaseRepository<Car>(context, logger), ICarRepository
{
    public async Task<(List<Car> Cars, int TotalCount)> GetPaginatedCarsAsync(CarFilter filter, int page, int pageSize)
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
                query = query.Where(c => c.Brand.ToLower().Contains(filter.Brand.ToLower()));

            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(c => c.Model.ToLower().Contains(filter.Model.ToLower()));

            if (filter.MinYear.HasValue)
                query = query.Where(c => c.Year >= filter.MinYear.Value);

            if (filter.MaxYear.HasValue)
                query = query.Where(c => c.Year <= filter.MaxYear.Value);

            if (!string.IsNullOrEmpty(filter.FuelType))
                query = query.Where(c => c.FuelType == filter.FuelType);

            if (filter is { StartDate: not null, EndDate: not null })
            {
                var startDate = DateOnly.FromDateTime(filter.StartDate.Value);
                var endDate = DateOnly.FromDateTime(filter.EndDate.Value);

                query = query.Where(c => !c.Offers.Any(o =>
                    o.Rental != null &&
                    o.Rental.RentalStatusId != RentalStatus.GetCompletedId() &&
                    o.StartDate <= endDate && o.EndDate >= startDate
                ));
            }

            int totalCount = await query.CountAsync();
            var cars = await query
                .OrderBy(car => car.CarId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (cars, totalCount);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching paginated cars with filters");
            throw new DatabaseOperationException("Failed to fetch paginated cars", ex);
        }
    }

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
                query = query.Where(c => c.Brand.ToLower().Contains(filter.Brand.ToLower()));

            if (!string.IsNullOrEmpty(filter.Model))
                query = query.Where(c => c.Model.ToLower().Contains(filter.Model.ToLower()));

            if (filter.MinYear.HasValue)
                query = query.Where(c => c.Year >= filter.MinYear.Value);

            if (filter.MaxYear.HasValue)
                query = query.Where(c => c.Year <= filter.MaxYear.Value);

            if (!string.IsNullOrEmpty(filter.FuelType))
                query = query.Where(c => c.FuelType == filter.FuelType);

            if (filter is { StartDate: not null, EndDate: not null })
            {
                var startDate = DateOnly.FromDateTime(filter.StartDate.Value);
                var endDate = DateOnly.FromDateTime(filter.EndDate.Value);

                // get rid of magic numbers
                query = query.Where(c => !c.Offers.Any(o =>
                    o.Rental != null &&
                    o.Rental.RentalStatusId != RentalStatus.GetCompletedId() &&
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