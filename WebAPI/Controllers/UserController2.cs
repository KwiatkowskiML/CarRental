using System.Security.Claims;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Mappers;
using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.Controllers
{   
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController2 : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper<Rental, RentalDTO> _rentalMapper;

        public UserController2(IUnitOfWork unitOfWork, IMapper<Rental, RentalDTO> rentalMapper)
        {
            _unitOfWork = unitOfWork;
            _rentalMapper = rentalMapper;
        }

        [HttpGet("/{customerId}/rentals2")]
        public async Task<IActionResult> GetUserRentals(int customerId)
        {
            try
            {
                var rentals = await _unitOfWork.RentalsRepository.GetUserRentalsAsync(customerId);
                var rentalDtos = rentals.Select(_rentalMapper.ToDto).ToList();
                return Ok(rentalDtos);
            }
            catch (DatabaseOperationException ex)
            {
                _unitOfWork.LogError(ex, "Error fetching user rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }

        [HttpGet("by-email2")]
        public async Task<IActionResult> GetUserIdByEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email address is required");
                }

                var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
                
                if (user == null)
                {
                    return NotFound($"User with email {email} not found");
                }

                return Ok(user.UserId);
            }
            catch (DatabaseOperationException ex)
            {
                _unitOfWork.LogError(ex, $"Error fetching user ID for email: {email}");
                return StatusCode(500, "An error occurred while fetching user ID");
            }
        }


        [HttpGet("current2")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized();
                }

                var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(new { 
                    userId = user.UserId,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName
                });
            }
            catch (DatabaseOperationException ex)
            {
                _unitOfWork.LogError(ex, "Error fetching current user");
                return StatusCode(500, "An error occurred while fetching user information");
            }
        }
    }

    
}