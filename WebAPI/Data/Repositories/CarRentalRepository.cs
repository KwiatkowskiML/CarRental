using Microsoft.EntityFrameworkCore;
using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;

namespace CarRental.WebAPI.Data.Repositories
{
    public class CarRentalRepository : ICarRentalRepository
    {
        private readonly CarRentalContext _context;
        private readonly ILogger<CarRentalRepository> _logger;

        public CarRentalRepository(
            CarRentalContext context,
            ILogger<CarRentalRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Car>> GetAllCarsAsync()
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.CarProvider)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all cars");
                throw new DatabaseOperationException("Failed to fetch cars", ex);
            }
        }

        public async Task<List<Car>> GetAvailableCarsAsync(CarFilter filter)
        {
            try
            {
                IQueryable<Car> query = _context.Cars
                    .Include(c => c.CarProvider)
                    .Where(c => c.Status == "available");

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
                    
                    query = query.Where(c => !c.Rentals.Any(r =>
                        r.Status != "cancelled" &&
                        ((r.StartDate <= endDate && r.EndDate >= startDate) ||
                         (r.StartDate >= startDate && r.StartDate <= endDate))));
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available cars with filters");
                throw new DatabaseOperationException("Failed to fetch available cars", ex);
            }
        }

        public async Task<Car?> GetCarByIdAsync(int carId)
        {
            try
            {
                return await _context.Cars
                    .Include(c => c.CarProvider)
                    .FirstOrDefaultAsync(c => c.CarId == carId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching car by id");
                throw new DatabaseOperationException($"Failed to fetch car with ID {carId}", ex);
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email");
                throw new DatabaseOperationException($"Failed to fetch user with email {email}", ex);
            }
        }

        public async Task<List<Rental>> GetUserRentalsAsync(int userId)
        {
            try
            {
                return await _context.Rentals
                    .Include(r => r.Car)
                    .Include(r => r.Customer)
                    .Where(r => r.Customer != null && r.Customer.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user rentals");
                throw new DatabaseOperationException($"Failed to fetch rentals for user {userId}", ex);
            }
        }

        public async Task<bool> CreateRentalAsync(Rental rental)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var car = await _context.Cars
                    .FirstOrDefaultAsync(c => c.CarId == rental.CarId);

                if (car == null || car.Status != "available")
                    return false;

                var hasOverlap = await _context.Rentals
                    .AnyAsync(r => r.CarId == rental.CarId &&
                                 r.Status != "cancelled" &&
                                 ((r.StartDate <= rental.EndDate && r.EndDate >= rental.StartDate) ||
                                  (r.StartDate >= rental.StartDate && r.StartDate <= rental.EndDate)));

                if (hasOverlap)
                    return false;

                _context.Rentals.Add(rental);
                car.Status = "rented";
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating rental");
                throw new DatabaseOperationException("Failed to create rental", ex);
            }
        }

        public async Task<bool> ProcessReturnAsync(Return carReturn)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var rental = await _context.Rentals
                    .Include(r => r.Car)
                    .FirstOrDefaultAsync(r => r.RentalId == carReturn.RentalId);

                if (rental == null || rental.Status != "active")
                    return false;

                _context.Returns.Add(carReturn);
                rental.Status = "completed";
                if (rental.Car != null)
                {
                    rental.Car.Status = "available";
                }
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing return");
                throw new DatabaseOperationException("Failed to process return", ex);
            }
        }
    }
}