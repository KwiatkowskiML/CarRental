using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        // ... rest of your implementation methods ...
    }
}