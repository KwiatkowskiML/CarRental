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
    public class UserController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUnitOfWork unitOfWork,
            ILogger<UserController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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

                var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(UserMapper.ToDto(user));
            }
            catch (DatabaseOperationException ex)
            {
                _unitOfWork.LogError(ex, "Error fetching current user");
                return StatusCode(500, "An error occurred while fetching user information");
            }
        }

    }
}