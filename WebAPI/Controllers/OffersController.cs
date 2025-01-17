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

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController(IUnitOfWork unitOfWork, IPriceCalculator priceCalculator) : ControllerBase
    {
        [HttpPost("get-offer")]
        public async Task<ActionResult<OfferDto>> GetOffer([FromBody] GetOfferRequest request)
        {
            try
            {
                var customer = await unitOfWork.UsersRepository.GetCustomerByUserIdAsync(request.UserId);

                if (customer == null)
                    return NotFound($"Customer with userId = {request.UserId} not found");

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
                Offer? existingOffer = await unitOfWork.OffersRepository.GetOfferAsync(offerFilter);
                if (existingOffer != null)
                    return Ok(OfferMapper.ToDto(existingOffer));

                // Checking car's accessibility for specified period
                var car = await unitOfWork.CarsRepository.GetCarByIdAsync(request.CarId);
                if (car == null)
                    return NotFound("Car not found");

                var filter = new CarFilter
                {
                    CarId = car.CarId,
                    StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue)
                };
                var availableCars = await unitOfWork.CarsRepository.GetCarsAsync(filter);
                if (availableCars.IsNullOrEmpty())
                {
                    return BadRequest("Car is not available for the selected dates");
                }

                // Checking insurance type
                var insurance = await unitOfWork.OffersRepository.GetInsuranceByIdAsync(request.InsuranceId);
                if (insurance == null)
                    return NotFound("Insurance package not found");

                decimal totalPrice = priceCalculator.CalculatePrice(car.BasePrice, insurance.Price,
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

                await unitOfWork.OffersRepository.CreateOfferAsync(offer);
                return Ok(OfferMapper.ToDto(offer));
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error calculating rental price");
                return StatusCode(500, "An error occurred while calculating the rental price");
            }
        }
    }
}