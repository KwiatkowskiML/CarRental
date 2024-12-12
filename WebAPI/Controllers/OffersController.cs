using System.Security.Claims;
using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Mappers;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.DTOs;
using WebAPI.filters;

namespace WebAPI.Controllers
{   
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<Offer, OfferDTO> _offerMapper;

        public OffersController(IUnitOfWork unitOfWork, IMapper<Offer, OfferDTO> offerMapper)
        {
            _unitOfWork = unitOfWork;
            _offerMapper = offerMapper;
        }

        [HttpPost("get-offer")]
        public async Task<ActionResult<OfferDTO>> GetOffer([FromBody] GetOfferRequest request)
        {
            try
            {
                var customer = await _unitOfWork.UsersRepository.GetCustomerByUserIdAsync(request.UserId);

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

                var existingOffer = await _unitOfWork.OffersRepository.GetOfferAsync(offerFilter);
                if (existingOffer != null)
                    return Ok(existingOffer);

                var car = await _unitOfWork.CarsRepository.GetCarByIdAsync(request.CarId);
                if (car == null)
                    return NotFound("Car not found");

                var filter = new CarFilter
                {
                    StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue)
                };

                var availableCars = await _unitOfWork.CarsRepository.GetCarsAsync(filter);
                if (availableCars.All(c => c.CarId != request.CarId))
                {
                    return BadRequest("Car is not available for the selected dates");
                }

                var insurance = await _unitOfWork.OffersRepository.GetInsuranceByIdAsync(request.InsuranceId);
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

                await _unitOfWork.OffersRepository.CreateOfferAsync(offer);
                var response = _offerMapper.ToDto(offer);
                return Ok(response);
            }
            catch (DatabaseOperationException ex)
            {
                _unitOfWork.LogError(ex, "Error calculating rental price");
                return StatusCode(500, "An error occurred while calculating the rental price");
            }
        }
        
        // TODO: relocate it
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