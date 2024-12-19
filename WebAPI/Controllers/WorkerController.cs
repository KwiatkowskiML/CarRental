using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;
using WebAPI.Mappers;
using WebAPI.Requests;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerController(IUnitOfWork unitOfWork) : ControllerBase
    {
        [HttpGet("rentals")]
        public async Task<IActionResult> GetUserRentals([FromQuery] RentalFilter request)
        {
            try
            {
                var rentals = await unitOfWork.RentalsRepository.GetRentalsAsync(request);
                var rentalDtos = rentals.Select(RentalMapper.ToDto).ToList();
                return Ok(rentalDtos);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }
        
        [HttpPost("accept-return")]
        public async Task<IActionResult> AcceptReturn([FromBody] AcceptReturnRequest request )
        {
            try
            {
                var rentalFilter = new RentalFilter() {RentalId = request.RentalId};
                var rentals = await unitOfWork.RentalsRepository.GetRentalsAsync(rentalFilter);
                
                if (rentals.Count == 0)
                    return NotFound($"Rental with RentalId {request.RentalId} not found");

                var rental = rentals.First();
                if (rental.RentalStatusId != RentalStatus.GetPendingId())
                    return BadRequest("Return is not pending");
                
                var completedReturn = await unitOfWork.RentalsRepository.ProcessReturn(request);
                return Ok(ReturnMapper.ToDto(completedReturn));
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }
    }
}