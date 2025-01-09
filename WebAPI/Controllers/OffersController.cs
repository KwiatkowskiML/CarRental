using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Data.DTOs;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;
using WebAPI.Mappers;
using WebAPI.PriceCalculators;
using WebAPI.Requests;
using ILogger = Google.Apis.Logging.ILogger;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICarRepository _carRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IPriceCalculator _priceCalculator;
        private readonly ILogger<OffersController> _logger;

        public OffersController(IPriceCalculator priceCalculator, ICustomerRepository customerRepository,
            ICarRepository carRepository, IOfferRepository offerRepository, ILogger<OffersController> logger)
        {
            _priceCalculator = priceCalculator;
            _customerRepository = customerRepository;
            _carRepository = carRepository;
            _offerRepository = offerRepository;
            _logger = logger;
        }

        [HttpPost("get-offer")]
        public async Task<ActionResult<OfferDto>> GetOffer([FromBody] GetOfferRequest request)
        {
            try
            {
                var customerFilter = new CustomerFilter() { UserId = request.UserId };
                var customersList = await _customerRepository.GetCustomersAsync(customerFilter);

                if (customersList.Count == 0)
                    return NotFound($"Customer with UserId {request.UserId} not found");

                if (customersList.Count > 1)
                    return StatusCode(500, $"Multiple customers found for UserId {request.UserId}");

                var customer = customersList.First();

                // TODO: consider filtering by dto
                var offerFilter = new OfferFilter
                {
                    CarId = request.CarId,
                    CustomerId = customer.CustomerId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    InsuranceId = request.InsuranceId,
                    HasGps = request.HasGps,
                    HasChildSeat = request.HasChildSeat
                };

                // Checking if offer has been already created
                Offer? existingOffer = await _offerRepository.GetOfferAsync(offerFilter);
                if (existingOffer != null)
                    return Ok(OfferMapper.ToDto(existingOffer));

                // Checking car's accessibility for specified period
                var car = await _carRepository.GetCarByIdAsync(request.CarId);
                if (car == null)
                    return NotFound("Car not found");

                var filter = new CarFilter
                {
                    CarId = car.CarId,
                    StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue)
                };
                var availableCars = await _carRepository.GetCarsAsync(filter);
                if (availableCars.IsNullOrEmpty())
                {
                    return BadRequest("Car is not available for the selected dates");
                }

                // Checking insurance type
                var insurance = await _offerRepository.GetInsuranceByIdAsync(request.InsuranceId);
                if (insurance == null)
                    return NotFound("Insurance package not found");

                decimal totalPrice = _priceCalculator.CalculatePrice(car.BasePrice, insurance.Price,
                    customer.DrivingLicenseYears, request);

                // create new offer
                var offer = new Offer
                {
                    CustomerId = customer.CustomerId,
                    Customer = customer,
                    CarId = request.CarId,
                    InsuranceId = request.InsuranceId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalPrice = totalPrice,
                    HasGps = request.HasGps,
                    HasChildSeat = request.HasChildSeat,
                    CreatedAt = DateTime.UtcNow
                };
                
                _logger.LogInformation("Offer Created: OfferID={OfferId}, CustomerId={CustomerId}, CarId={CarId}, InsuranceId={InsuranceId}, StartDate={StartDate}, EndDate={EndDate}, TotalPrice={TotalPrice}, HasGps={HasGps}, HasChildSeat={HasChildSeat}, CreatedAt={CreatedAt}",
                    offer.OfferId, offer.CustomerId, offer.CarId, offer.InsuranceId, offer.StartDate, offer.EndDate, offer.TotalPrice, offer.HasGps, offer.HasChildSeat, offer.CreatedAt);


                await _offerRepository.CreateOfferAsync(offer);
                return Ok(OfferMapper.ToDto(offer));
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error calculating rental price");
                return StatusCode(500, "An error occurred while calculating the rental price");
            }
        }
    }
}