using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using WebAPI.filters;

namespace CarRental.WebAPI.Controllers;  // Make sure this matches your other controllers

[ApiController]
[Route("api/[controller]")]
public class RentalConfirmationController : ControllerBase
{
    private readonly IRentalConfirmationService _confirmationService;
    private readonly ICarRentalRepository _repository;
    private readonly ILogger<RentalConfirmationController> _logger;

    public RentalConfirmationController(
        IRentalConfirmationService confirmationService,
        ICarRentalRepository repository,
        ILogger<RentalConfirmationController> logger)
    {
        _confirmationService = confirmationService;
        _repository = repository;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendConfirmationEmail([FromBody] SendConfirmationEmailRequest request)
    {
        _logger.LogInformation("Received confirmation request: {@Request}", request);
        
        try
        {
            if (request.OfferId <= 0)
            {
                return BadRequest($"Invalid OfferId: {request.OfferId}");
            }

            // Get current user from the token
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("No email claim found in token");
            }

            var user = await _repository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User not found for email: {email}");
            }

            // Get customer record for the user
            var customer = await _repository.GetCustomerByUserId(user.UserId);
            if (customer == null)
            {
                return NotFound($"Customer record not found for user {user.UserId}");
            }

            // Verify the offer exists and belongs to this customer
            var filter = new OfferFilter 
            { 
                OfferId = request.OfferId,
                CustomerId = customer.CustomerId  // Use the customer ID from the user's record
            };

            _logger.LogInformation("Checking offer with filter: {@Filter}", filter);
            var offer = await _repository.GetOffer(filter);
            
            if (offer == null)
            {
                _logger.LogWarning("Offer not found or unauthorized. OfferId: {OfferId}, CustomerId: {CustomerId}", 
                    request.OfferId, customer.CustomerId);
                return BadRequest($"Offer not found or unauthorized access");
            }

            // Send confirmation email
            await _confirmationService.SendConfirmationEmail(
                request.OfferId,
                customer.CustomerId,  // Use customer ID instead of user ID
                user.Email,
                $"{user.FirstName} {user.LastName}");

            return Ok(new { message = "Confirmation email sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing confirmation request: {@Request}", request);
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}

public class SendConfirmationEmailRequest
{
    public int OfferId { get; set; }
}