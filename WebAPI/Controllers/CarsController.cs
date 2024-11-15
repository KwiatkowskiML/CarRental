using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;

namespace CarRental.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarRentalRepository _repository;
        private readonly ILogger<CarsController> _logger;

        public CarsController(
            ICarRentalRepository repository,
            ILogger<CarsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars([FromQuery] CarFilter filter)
        {
            try
            {
                var cars = await _repository.GetAvailableCarsAsync(filter);
                
                var carDTOs = cars.Select(c => new CarDTO
                {
                    CarId = c.CarId,
                    LicensePlate = c.LicensePlate,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year,
                    Status = c.Status,
                    Location = c.Location,
                    EngineCapacity = c.EngineCapacity,
                    Power = c.Power,
                    FuelType = c.FuelType,
                    Description = c.Description,
                    CarProvider = c.CarProvider != null ? new CarProviderDTO
                    {
                        CarProviderId = c.CarProvider.CarProviderId,
                        Name = c.CarProvider.Name,
                        ContactEmail = c.CarProvider.ContactEmail,
                        ContactPhone = c.CarProvider.ContactPhone
                    } : null
                }).ToList();

                return Ok(carDTOs);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error fetching cars");
                return StatusCode(500, "An error occurred while fetching cars");
            }
        }

        [HttpGet("user/{userId}/rentals")]
        public async Task<IActionResult> GetUserRentals(int userId)
        {
            try
            {
                var rentals = await _repository.GetUserRentalsAsync(userId);
                return Ok(rentals);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error fetching user rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }
    }
}