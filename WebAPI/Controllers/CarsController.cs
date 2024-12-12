using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using WebAPI.DTOs;
using CarRental.WebAPI.Data.Models;
using WebAPI.Data.Mappers;
using WebAPI.Data.Maps;
using WebAPI.filters;

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

        [HttpPost("get-offer")]
        public async Task<ActionResult<OfferDTO>> GetOffer([FromBody] GetOfferRequest request)
        {
            try
            {
                var customer = await _repository.GetCustomerByUserId(request.UserId);

                if (customer == null)
                    return NotFound($"Customer with userId = {request.UserId} not found");

                var offerFilter = new OfferFilter{
                    CarId = request.CarId,
                    CustomerId = customer.CustomerId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    InsuranceId = request.InsuranceId,
                    HasGps = request.HasGps,
                    HasChildSeat = request.HasChildSeat
                };

                var existingOffer = await _repository.GetOffer(offerFilter);
                if (existingOffer != null)
                    return Ok(existingOffer);

                var car = await _repository.GetCarByIdAsync(request.CarId);
                if (car == null)
                    return NotFound("Car not found");

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

                var offerMapper = new OfferMapper();
                var response = offerMapper.ToDto(offer);

                return Ok(response);

            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error calculating rental price");
                return StatusCode(500, "An error occurred while calculating the rental price");
            }
        }

        [HttpPost("choose-offer")]
        public async Task<ActionResult> ChooseOffer([FromBody] ChooseOfferRequest request)
        {
            try
            {
                var customer = await _repository.GetCustomerByUserId(request.UserId);

                if (customer == null)
                    return NotFound($"Customer with userId = {request.UserId} not found");

                var offerFilter = new OfferFilter{
                    OfferId = request.OfferId,
                    CustomerId = customer.CustomerId
                };

                var offer = await _repository.GetOffer(offerFilter);
                if (offer == null)
                    return NotFound($"Offer with ID {request.OfferId} not found for this customer");

                return Ok();
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error choosing offer");
                return StatusCode(500, "An error occurred while choosing offer");
            }
        }

        [HttpPost("create-rental/{offerId:int}/{userId:int}")]
        public async Task<ActionResult<RentalDTO>> CreateRental(int offerId, int userId)
        {
            try
            {
                // First verify if the user exists and is authorized to create this rental
                var customer = await _repository.GetCustomerByUserId(userId);
                if (customer == null)
                    return NotFound($"Customer with userId = {userId} not found");

                // Get the offer to verify it belongs to this user
                var offerFilter = new OfferFilter
                {
                    OfferId = offerId,
                    CustomerId = customer.CustomerId
                };

                var offer = await _repository.GetOffer(offerFilter);
                if (offer == null)
                    return NotFound($"Offer with ID {offerId} and CustomerID related to UserID {userId} not found for this customer");

                var rental = await _repository.CreateRentalFromOfferAsync(offerId);
                if (rental == null)
                    return NotFound($"Failed to create rental for offer {offerId}");

                return Ok(rental);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error creating rental");
                return StatusCode(500, "An error occurred while creating the rental");
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