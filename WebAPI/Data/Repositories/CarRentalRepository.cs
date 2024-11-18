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

        public async Task<Customer?> GetCustomerByUserId(int userId)
        {
            try
            {
                return await _context.Customers
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer by userId");
                throw new DatabaseOperationException($"Failed to fetch customer with userId {userId}", ex);
            }
        }

        public async Task<List<RentalDTO>> GetUserRentalsAsync(int userId)
        {
            try
            {
                var rentals = await _context.Rentals
                    .Include(r => r.Insurance) // Optional: Include related Insurance details
                    .Where(r => r.Customer != null && r.Customer.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                var rentalDTOs = rentals.Select(r => new RentalDTO
                {
                    RentalId = r.RentalId,
                    CustomerId = r.CustomerId,
                    CarId = r.CarId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    HasGps = r.HasGps,
                    HasChildSeat = r.HasChildSeat,
                    InsuranceId = r.InsuranceId,
                    Insurance = r.Insurance
                }).ToList();

                return rentalDTOs;
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

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<Insurance?> GetInsuranceByIdAsync(int insuranceId)
        {
            try
            {
                return await _context.Insurances
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.InsuranceId == insuranceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching insurance by id {InsuranceId}", insuranceId);
                throw new DatabaseOperationException($"Failed to fetch insurance with ID {insuranceId}", ex);
            }
        }

        public async Task CreateOfferAsync(Offer offer)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var car = await _context.Cars
                    .FirstOrDefaultAsync(c => c.CarId == offer.CarId);

                if (car == null)
                    throw new InvalidOperationException($"Car with ID {offer.CarId} not found");

                var hasConflict = await _context.Rentals
                    .AnyAsync(r => r.CarId == offer.CarId &&
                                r.Status != "cancelled" &&
                                ((r.StartDate <= offer.EndDate && r.EndDate >= offer.StartDate) ||
                                (r.StartDate >= offer.StartDate && r.StartDate <= offer.EndDate)));

                if (hasConflict)
                    throw new InvalidOperationException("Car is not available for the selected dates");

                if (offer.CustomerId.HasValue)
                {
                    var customerExists = await _context.Customers
                        .AnyAsync(c => c.CustomerId == offer.CustomerId);

                    if (!customerExists)
                        throw new InvalidOperationException($"Customer with ID {offer.CustomerId} not found");
                }

                // Validate the insurance exists
                if (offer.InsuranceId.HasValue)
                {
                    var insuranceExists = await _context.Insurances
                        .AnyAsync(i => i.InsuranceId == offer.InsuranceId);

                    if (!insuranceExists)
                        throw new InvalidOperationException($"Insurance with ID {offer.InsuranceId} not found");
                }

                // Set creation timestamp if not already set
                if (!offer.CreatedAt.HasValue)
                    offer.CreatedAt = DateTime.UtcNow;

                // Add the offer to the context
                await _context.Offers.AddAsync(offer);
                await _context.SaveChangesAsync();

                await _context.Entry(offer)
                    .Reference(o => o.Insurance)
                    .LoadAsync();

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating offer for car {CarId}", offer.CarId);
                throw new DatabaseOperationException("Failed to create offer", ex);
            }
        }
    }
}