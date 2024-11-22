using Microsoft.EntityFrameworkCore;
using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using WebAPI.DTOs;
using WebAPI.Data.Maps;
using WebAPI.filters;

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
                    
                    query = query.Where(c => !c.Offers.Any(o => 
                        o.Rental != null && 
                        o.Rental.Status != "cancelled" &&
                        ((o.StartDate <= endDate && o.EndDate >= startDate) ||
                        (o.StartDate >= startDate && o.StartDate <= endDate))
                    ));
                }

                return await query.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available cars with filters");
                throw new DatabaseOperationException("Failed to fetch available cars", ex);
            }
        }

        public async Task<OfferDTO?> GetOffer(OfferFilter filter)
        {
            try
            {
                _logger.LogInformation($"Searching for offer - OfferId: {filter.OfferId}, CustomerId: {filter.CustomerId}");

                var query = _context.Offers
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
                _logger.LogInformation($"Generated SQL: {sql}");

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
                _logger.LogError(ex, "Error fetching offer with filter");
                throw new DatabaseOperationException("Failed to fetch offer with specified filter", ex);
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
                var query = _context.Rentals
                    .Include(r => r.Offer)
                        .ThenInclude(o => o.Customer)
                    .Include(r => r.Offer)
                        .ThenInclude(o => o.Car)
                            .ThenInclude(c => c.CarProvider)
                    .Where(r => r.Offer.Customer != null && r.Offer.Customer.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt);

                var rentals = await query.ToListAsync();
                var rentalDTOs = rentals.Select(r => Mapper.RentalToDTO(r)).ToList();

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
            // using var transaction = await _context.Database.BeginTransactionAsync();
            // try
            // {
            //     var car = await _context.Cars
            //         .FirstOrDefaultAsync(c => c.CarId == rental.CarId);

            //     if (car == null || car.Status != "available")
            //         return false;

            //     var hasOverlap = await _context.Rentals
            //         .AnyAsync(r => r.CarId == rental.CarId &&
            //                      r.Status != "cancelled" &&
            //                      ((r.StartDate <= rental.EndDate && r.EndDate >= rental.StartDate) ||
            //                       (r.StartDate >= rental.StartDate && r.StartDate <= rental.EndDate)));

            //     if (hasOverlap)
            //         return false;

            //     _context.Rentals.Add(rental);
            //     car.Status = "rented";
                
            //     await _context.SaveChangesAsync();
            //     await transaction.CommitAsync();
                
            //     return true;
            // }
            // catch (Exception ex)
            // {
            //     await transaction.RollbackAsync();
            //     _logger.LogError(ex, "Error creating rental");
            //     throw new DatabaseOperationException("Failed to create rental", ex);
            // }

            return false;
        }

        public async Task<bool> ProcessReturnAsync(Return carReturn)
        {
            // using var transaction = await _context.Database.BeginTransactionAsync();
            // try
            // {
            //     var rental = await _context.Rentals
            //         .Include(r => r.Car)
            //         .FirstOrDefaultAsync(r => r.RentalId == carReturn.RentalId);

            //     if (rental == null || rental.Status != "active")
            //         return false;

            //     _context.Returns.Add(carReturn);
            //     rental.Status = "completed";
            //     if (rental.Car != null)
            //     {
            //         rental.Car.Status = "available";
            //     }
                
            //     await _context.SaveChangesAsync();
            //     await transaction.CommitAsync();
                
            //     return true;
            // }
            // catch (Exception ex)
            // {
            //     await transaction.RollbackAsync();
            //     _logger.LogError(ex, "Error processing return");
            //     throw new DatabaseOperationException("Failed to process return", ex);
            // }

            throw new NotImplementedException("Processing a retun not implemented yet");
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
                    .AnyAsync(r => r.Offer.CarId == offer.CarId &&
                                r.Status != "cancelled" &&
                                ((r.Offer.StartDate <= offer.EndDate && r.Offer.EndDate >= offer.StartDate) ||
                                (r.Offer.StartDate >= offer.StartDate && r.Offer.StartDate <= offer.EndDate)));

                if (hasConflict)
                    throw new InvalidOperationException("Car is not available for the selected dates");


                var customerExists = await _context.Customers
                    .AnyAsync(c => c.CustomerId == offer.CustomerId);

                if (!customerExists)
                    throw new InvalidOperationException($"Customer with ID {offer.CustomerId} not found");


                var insuranceExists = await _context.Insurances
                    .AnyAsync(i => i.InsuranceId == offer.InsuranceId);

                if (!insuranceExists)
                    throw new InvalidOperationException($"Insurance with ID {offer.InsuranceId} not found");

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

        public async Task<RentalDTO?> CreateRentalFromOfferAsync(int offerId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var offer = await _context.Offers
                    .Include(o => o.Car)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.OfferId == offerId);

                if (offer == null)
                    return null;

                if (offer.Car.Status != "available")
                    throw new InvalidOperationException("Car is not available for rental");

                var hasOverlap = await _context.Rentals
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
                _context.Rentals.Add(rental);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Load necessary navigation properties for the DTO
                await _context.Entry(rental)
                    .Reference(r => r.Offer)
                    .Query()
                    .Include(o => o.Car)
                        .ThenInclude(c => c.CarProvider)
                    .Include(o => o.Customer)
                    .Include(o => o.Insurance)
                    .LoadAsync();

                return Mapper.RentalToDTO(rental);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating rental for offer {OfferId}", offerId);
                throw new DatabaseOperationException("Failed to create rental", ex);
            }
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                throw new DatabaseOperationException("Failed to create customer", ex);
            }
        }
        public async Task<RentalDTO?> GetRentalByOfferId(int offerId)
        {
            try
            {
                var rental = await _context.Rentals
                    .Include(r => r.Offer)
                        .ThenInclude(o => o.Car)
                            .ThenInclude(c => c.CarProvider)
                    .Include(r => r.Offer)
                        .ThenInclude(o => o.Customer)
                    .Include(r => r.Offer)
                        .ThenInclude(o => o.Insurance)
                    .FirstOrDefaultAsync(r => r.OfferId == offerId);

                if (rental == null)
                {
                    return null;
                }

                _logger.LogInformation("Found rental for offerId {OfferId}: {@Rental}", offerId, rental);
                return Mapper.RentalToDTO(rental);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rental by offerId {OfferId}", offerId);
                throw new DatabaseOperationException($"Failed to get rental for offer {offerId}", ex);
            }
        }

    }
}