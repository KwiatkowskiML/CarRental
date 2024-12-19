using System.Security.Claims;
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
    public class CustomerController(IUnitOfWork unitOfWork) : ControllerBase
    {
        [HttpGet("{customerId}/rentals")]
        public async Task<IActionResult> GetCustomerRentals(int customerId)
        {
            try
            {
                var rentalFilter = new RentalFilter() {CustomerId = customerId};
                var rentals = await unitOfWork.RentalsRepository.GetRentalsAsync(rentalFilter);
                var rentalDtos = rentals.Select(RentalMapper.ToDto).ToList();
                return Ok(rentalDtos);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching user rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetCustomerId([FromQuery] int userId)
        {
            try
            {
                var customer = await unitOfWork.UsersRepository.GetCustomerByUserIdAsync(userId);

                if (customer == null)
                {
                    return NotFound($"Customer with UserId {userId} not found");
                }

                return Ok(CustomerMapper.ToDto(customer));
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, $"Error fetching CustomerId for UserId: {userId}");
                return StatusCode(500, "An error occurred while fetching CustomerId");
            }
        }
    }
}