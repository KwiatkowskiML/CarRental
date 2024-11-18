using Microsoft.AspNetCore.Mvc;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace CarRental.WebAPI.Controllers
{   
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ICarRentalRepository _repository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            ICarRentalRepository repository,
            ILogger<UserController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("/{userId}/rentals")]
        public async Task<IActionResult> GetUserRentals(int userId)
        {
            try
            {
                var rentals = await _repository.GetUserRentalsAsync(userId);
                return Ok(rentals);
            }
            catch (DatabaseOperationException ex)
            {
                _logger.LogError(ex, "Error fetching user rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }
    }
}