using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;
using WebAPI.Mappers;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerController(IUnitOfWork unitOfWork) : ControllerBase
    {
        [HttpGet("/rentals")]
        public async Task<IActionResult> GetUserRentals()
        {
            try
            {
                var rentalFilter = new RentalFilter();
                var rentals = await unitOfWork.RentalsRepository.GetRentalsAsync(rentalFilter);
                var rentalDtos = rentals.Select(RentalMapper.ToDto).ToList();
                return Ok(rentalDtos);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }
    }
}