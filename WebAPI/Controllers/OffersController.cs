using System.Security.Claims;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Data.Mappers;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.DTOs;
using WebAPI.filters;
using WebAPI.HelperClasses;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OffersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("get-offer")]
        public async Task<ActionResult<OfferDTO>> GetOffer([FromBody] GetOfferRequest request)
        {
            try
            {
                var customer = await _unitOfWork.UsersRepository.GetCustomerByUserIdAsync(request.UserId);

                if (customer == null)
                    return NotFound($"Customer with userId = {request.UserId} not found");

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
                Offer? existingOffer = await _unitOfWork.OffersRepository.GetOfferAsync(offerFilter);
                if (existingOffer != null)
                    return Ok(OfferMapper.ToDto(existingOffer));

                // Checking car's accessibility for specified period
                var car = await _unitOfWork.CarsRepository.GetCarByIdAsync(request.CarId);
                if (car == null)
                    return NotFound("Car not found");

                var filter = new CarFilter
                {
                    CarId = car.CarId,
                    StartDate = request.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = request.EndDate.ToDateTime(TimeOnly.MinValue)
                };
                var availableCars = await _unitOfWork.CarsRepository.GetCarsAsync(filter);
                if (availableCars.IsNullOrEmpty())
                {
                    return BadRequest("Car is not available for the selected dates");
                }

                // Checking insurance type
                var insurance = await _unitOfWork.OffersRepository.GetInsuranceByIdAsync(request.InsuranceId);
                if (insurance == null)
                    return NotFound("Insurance package not found");

                decimal totalPrice = PriceCalculator.GetPrice(car.BasePrice, insurance.Price,
                    customer.DrivingLicenseYears, request);

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
                return Ok(OfferMapper.ToDto(offer));
            }
            catch (DatabaseOperationException ex)
            {
                _unitOfWork.LogError(ex, "Error calculating rental price");
                return StatusCode(500, "An error occurred while calculating the rental price");
            }
        }

        // TODO: look into it
        [HttpPost("choose-offer")]
        public async Task<ActionResult> ChooseOffer([FromBody] ChooseOfferRequest request)
        {
            try
            {
                var customer = await _unitOfWork.UsersRepository.GetCustomerByUserIdAsync(request.UserId);

                if (customer == null)
                    return NotFound($"Customer with userId = {request.UserId} not found");

                var offerFilter = new OfferFilter
                {
                    OfferId = request.OfferId,
                    CustomerId = customer.CustomerId
                };

                var offer = await _unitOfWork.OffersRepository.GetOfferAsync(offerFilter);
                if (offer == null)
                    return NotFound($"Offer with ID {request.OfferId} not found for this customer");

                return Ok();
            }
            catch (DatabaseOperationException ex)
            {
                _unitOfWork.LogError(ex, "Error choosing offer");
                return StatusCode(500, "An error occurred while choosing offer");
            }
        }
    }
}