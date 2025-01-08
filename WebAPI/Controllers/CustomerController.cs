using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;
using WebAPI.Mappers;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController(IUnitOfWork unitOfWork, IEmailService emailService) : ControllerBase
    {
        [HttpGet("{customerId}/rentals")]
        public async Task<IActionResult> GetCustomerRentals(int customerId)
        {
            try
            {
                var rentalFilter = new RentalFilter() { CustomerId = customerId };
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
        
        
        [HttpPost("return/{rentalId}/{customerId}")]
        public async Task<IActionResult> ProcessReturn(int rentalId, int customerId)
        {
            try
            {
                var customer = await unitOfWork.UsersRepository.GetCustomerAsync(customerId);
                if (customer == null)
                {
                    return NotFound($"Customer with CustomerId {customerId} not found");
                }

                // Input data validation
                var filter = new RentalFilter() { CustomerId = customerId, RentalId = rentalId };
                var rentals = await unitOfWork.RentalsRepository.GetRentalsAsync(filter);
                if (rentals.Count == 0)
                {
                    return NotFound($"Rental with RentalId {rentalId} not found for CustomerId {customerId}");
                }
                if (rentals.Count > 1)
                {
                    return StatusCode(500, $"Multiple rentals found for RentalId {rentalId} and CustomerId {customerId}");
                }
                
                var rental = rentals.First();
                if (rental.RentalStatusId != RentalStatus.GetConfirmedId())
                {
                    return StatusCode(500, $"Rental with RentalId {rentalId} is not in Confirmed status");
                }

                // Process return
                var result = await unitOfWork.RentalsRepository.InitReturn(rentalId);
                if (!result)
                {
                    return StatusCode(500, $"Error initializing return for RentalId: {rentalId} and CustomerId: {customerId}");
                }
                
                // If return has been correctly initialized, send an email to the customer
                var userFilter = new UserFilter() { UserId = customer.UserId };
                var users = await unitOfWork.UsersRepository.GetUsersAsync(userFilter);
                if (users.Count == 0)
                    return NotFound("User not found");
                if (users.Count > 1)
                    return BadRequest("Multiple users found for the same ID");
                var user = users[0];
                await emailService.SendReturnProcessInitiatedEmail(user.Email, user.FirstName);
                
                return Ok(result);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, $"Error initializing return for RentalId: {rentalId} and CustomerId: {customerId}");
                return StatusCode(500, "An error occurred while initializing the return");
            }
        }
        
    }
}