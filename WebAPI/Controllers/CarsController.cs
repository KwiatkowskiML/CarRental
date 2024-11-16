using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using CarRental.WebAPI.Data.Models;
using WebAPI.Data.Models;

namespace CarRental.WebAPI.Controllers
{   
    //[Authorize]
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

        [HttpPost("get-offer")]
        public async Task<ActionResult<RentalOfferResponse>> CalculateRental([FromBody] RentalCalculationRequest request)
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

                // calculating base price
                var duration = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;
                var basePrice = car.BasePrice * duration;
                basePrice = (basePrice / customer.DrivingLicenseYears) + basePrice;

                // calculating insurance cost
                decimal insuranceCost = request.InsuranceType switch
                {
                    InsuranceTypeEnum.StandardInsurance => basePrice * 0.1m, 
                    InsuranceTypeEnum.NoInsurance => 0,    
                    _ => 0m
                };

                // other additional costs calculation
                decimal gpsPrice = request.HasGps ? 3.0m * duration : 0m;
                decimal childSeatPrice = request.HasChildSeat ? 2.0m * duration : 0m;

                decimal totalPrice = basePrice + insuranceCost + gpsPrice + childSeatPrice;

                var response = new RentalOfferResponse
                {
                    TotalPrice = totalPrice,
                    InsuranceType = request.InsuranceType.ToString(),
                    HasGps = request.HasGps,
                    HasChildSeat = request.HasChildSeat,
                    CarProvider = new CarProviderDTO
                    {
                        CarProviderId = car.CarProviderId,
                        Name = car.CarProvider!.Name,
                        ContactEmail = car.CarProvider.ContactEmail,
                        ContactPhone = car.CarProvider.ContactPhone
                    }
                };

                return Ok(response);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error calculating rental price");
                return StatusCode(500, "An error occurred while calculating the rental price");
            }
        }
    }
}