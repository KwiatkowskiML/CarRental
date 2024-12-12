using System.Security.Claims;
using CarRental.WebAPI.Controllers;
using CarRental.WebAPI.Data.Repositories.Interfaces;
using CarRental.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Mappers;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.filters;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalConfirmationService _confirmationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RentalConfirmationController> _logger;

    public RentalsController(
        IRentalConfirmationService confirmationService,
        IUnitOfWork unitOfWork,
        ILogger<RentalConfirmationController> logger)
    {
        _confirmationService = confirmationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost("send-confirmation")]
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

            var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User not found for email: {email}");
            }

            // Get customer record for the user
            var customer = await _unitOfWork.UsersRepository.GetCustomerByUserIdAsync(user.UserId);
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
            var offer = await _unitOfWork.OffersRepository.GetOfferAsync(filter);
            
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

    [HttpGet("validate-token")]
    public async Task<IActionResult> ValidateToken([FromQuery] string token)
    {
        _logger.LogInformation("Validate endpoint called");
        _logger.LogInformation("Received token: {Token}", token);

        try
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Received null or empty token");
                return BadRequest("Token is required");
            }

            // Try to decode the token
            var decodedToken = Uri.UnescapeDataString(token);
            _logger.LogInformation("Decoded token: {DecodedToken}", decodedToken);

            var (isValid, offerId, userId) = _confirmationService.ValidateConfirmationToken(decodedToken);
            _logger.LogInformation("Validation result - IsValid: {IsValid}, OfferId: {OfferId}, UserId: {UserId}",
                isValid, offerId, userId);
            
            if (!isValid)
                return BadRequest("Invalid or expired confirmation link");

            // TODO: block unautorized users

            // Verify the current user matches the token's user
            // var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            // var user = await _repository.GetUserByEmailAsync(currentUserEmail);
            
            // if (user == null || user.UserId != userId)
            // {
            //     return Unauthorized("This confirmation link is for a different user");
            // }

            // Get the offer details
            var filter = new OfferFilter { OfferId = offerId };
            var offer = await _unitOfWork.OffersRepository.GetOfferAsync(filter);
            
            if (offer == null)
                return NotFound("Offer not found");

            // Return offer details for confirmation page
            return Ok(offer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(500, "An error occurred while validating the token");
        }
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmRental([FromQuery] string token)
    {
        try
        {
            var (isValid, offerId, userId) = _confirmationService.ValidateConfirmationToken(token);

            if (!isValid)
                return BadRequest("Invalid or expired confirmation link");

            // Check if rental already exists for this offer
            var existingRental = await _unitOfWork.RentalsRepository.GetRentalByOfferIdAsync(offerId);
            if (existingRental != null)
            {
                return StatusCode(409, "Rental has already been confirmed");
            }

            // Create the rental
            var rental = await _unitOfWork.RentalsRepository.CreateRentalFromOfferAsync(offerId);
            if (rental == null)
                return BadRequest("Failed to create rental");

            var result = RentalMapper.ToDto(rental);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming rental");
            return StatusCode(500, "An error occurred while confirming the rental");
        }
    }
}