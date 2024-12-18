using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.Mappers;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUnitOfWork unitOfWork) : ControllerBase
    {
        [HttpGet("/{customerId}/rentals")]
        public async Task<IActionResult> GetUserRentals(int customerId)
        {
            try
            {
                var rentals = await unitOfWork.RentalsRepository.GetCustomerRentalsAsync(customerId);
                var rentalDtos = rentals.Select(RentalMapper.ToDto).ToList();
                return Ok(rentalDtos);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching user rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetUserIdByEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email address is required");
                }

                var user = await unitOfWork.UsersRepository.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return NotFound($"User with email {email} not found");
                }

                return Ok(user.UserId);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, $"Error fetching user ID for email: {email}");
                return StatusCode(500, "An error occurred while fetching user ID");
            }
        }


        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized();
                }

                var user = await unitOfWork.UsersRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // TODO: return dto instead
                return Ok(new
                {
                    userId = user.UserId,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName
                });
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching current user");
                return StatusCode(500, "An error occurred while fetching user information");
            }
        }
    }
}