using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using CarRental.WebAPI.Data.Models;
using WebAPI.Data.Maps;

namespace CarRental.WebAPI.Controllers
{   
    [Authorize]
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

        // todo: if user is a client and the dates are specified, do not return cars that are reserved for the period
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
        
        [Authorize]
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

        [Authorize]
        [HttpPost("get-offer")]
        public async Task<ActionResult<OfferDTO>> CalculateRental([FromBody] RentalCalculationRequest request)
        {
            try
            {
                var car = await _repository.GetCarByIdAsync(request.CarId);
                if (car == null)
                {
                    return NotFound("Car not found");
                }

                // checking car availability in the selected period
                var filter = new CarFilter
                {
                    StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue)
                };
                var availableCars = await _repository.GetAvailableCarsAsync(filter);
                if (!availableCars.Any(c => c.CarId == request.CarId))
                {
                    return BadRequest("Car is not available for the selected dates");
                }

                // fetching customer
                var customer = await _repository.GetCustomerByUserId(request.UserId);
                if (customer == null)
                {
                    return NotFound("Customer not found");
                }

                var insurance = await _repository.GetInsuranceByIdAsync(request.InsuranceId);
                if (insurance == null)
                    return NotFound("Insurance package not found");

                // calculate total price
                decimal totalPrice = CalculateTotalPrice(
                    car.BasePrice,
                    request.StartDate,
                    request.EndDate,
                    insurance.Price,
                    request.HasGps,
                    request.HasChildSeat,
                    customer.DrivingLicenseYears
                );

                // create new offer
                var offer = new Offer
                {
                    CustomerId = customer.CustomerId,
                    CarId = request.CarId,
                    InsuranceId = request.InsuranceId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalPrice = totalPrice,
                    HasGps = request.HasGps,
                    HasChildSeat = request.HasChildSeat,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.CreateOfferAsync(offer);

                var response = Mapper.OfferToOfferDto(offer);

                return Ok(response);

            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error calculating rental price");
                return StatusCode(500, "An error occurred while calculating the rental price");
            }
        }

        private decimal CalculateTotalPrice(
            decimal basePrice,
            DateOnly startDate,
            DateOnly endDate,
            decimal insurancePrice,
            bool hasGps,
            bool hasChildSeat,
            int yearsOfExperience)
        {
            int numberOfDays = endDate.DayNumber - startDate.DayNumber + 1;
            decimal totalPrice = basePrice * numberOfDays;
            totalPrice += totalPrice / yearsOfExperience;

            totalPrice += insurancePrice * numberOfDays;

            if (hasGps)
                totalPrice += 10.00m * numberOfDays;
            if (hasChildSeat)
                totalPrice += 15.00m * numberOfDays;

            return totalPrice;
        }
    }
}