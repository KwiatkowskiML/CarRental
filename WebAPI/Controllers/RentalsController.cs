using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.filters;
using WebAPI.Mappers;
using WebAPI.Exceptions;
using WebAPI.Requests;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalConfirmationService _confirmationService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RentalsController> _logger;

    public RentalsController(
        IRentalConfirmationService confirmationService,
        IUnitOfWork unitOfWork,
        ILogger<RentalsController> logger, IEmailService emailService)
    {
        _confirmationService = confirmationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _emailService = emailService;
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
        try
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required");
            
            // Token decoding
            var decodedToken = Uri.UnescapeDataString(token);
            var (isValid, offerId, customerId) = _confirmationService.ValidateConfirmationToken(decodedToken);
            _logger.LogInformation("Validation result - IsValid: {IsValid}, OfferId: {OfferId}, CustomerId: {customerId}",
                isValid, offerId, customerId);
            
            if (!isValid)
                return BadRequest("Invalid or expired confirmation link");
            
            // Verify the current user matches the token's user
            var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(currentUserEmail))
                return NotFound("No current user email found");
            _logger.LogInformation($"Current user's email: {currentUserEmail}");
            
            var user = await _unitOfWork.UsersRepository.GetUserByEmailAsync(currentUserEmail); 
            if (user == null)
                return NotFound($"User with email address {currentUserEmail} not found");
            
            var customer = await _unitOfWork.UsersRepository.GetCustomerByUserIdAsync(user.UserId);
            if (customer == null)
                return BadRequest("Customer record not found");
            
            if (customerId != customer.CustomerId)
                return Unauthorized("This confirmation link is for a different user");
            
            // Get the offer details
            var filter = new OfferFilter { OfferId = offerId };
            var offer = await _unitOfWork.OffersRepository.GetOfferAsync(filter);
            
            if (offer == null)
                return NotFound("Offer not found");

            // Return offer details for confirmation page
            return Ok(OfferMapper.ToDto(offer));
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
            var (isValid, offerId, customerId) = _confirmationService.ValidateConfirmationToken(token);

            if (!isValid)
                return BadRequest("Invalid or expired confirmation link");

            // Check if rental already exists for this offer
            var existingRental = await _unitOfWork.RentalsRepository.GetRentalByOfferIdAsync(offerId);
            if (existingRental != null)
            {
                return StatusCode(409, "Rental has already been confirmed");
            }
            
            // Get user information
            var customer = await _unitOfWork.UsersRepository.GetCustomerAsync(customerId);
            if (customer == null)
                return NotFound("Customer not found");
            
            var userFilter = new UserFilter() { UserId = customer.UserId };
            var users = await _unitOfWork.UsersRepository.GetUsersAsync(userFilter);
            
            if (users.Count == 0)
                return NotFound("User not found");
            
            if (users.Count > 1)
                return BadRequest("Multiple users found for the same ID");
            
            var user = users[0];

            // Create the rental
            var rental = await _unitOfWork.RentalsRepository.CreateRentalFromOfferAsync(offerId);
            if (rental == null)
                return BadRequest("Failed to create rental");
            
            // Since rental has been successfully created, sending success email
            await _emailService.SendRentalSuccessEmail(user.Email, user.FirstName, "placeholder");

            var result = RentalMapper.ToDto(rental);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming rental");
            return StatusCode(500, "An error occurred while confirming the rental");
        }
    }

    [HttpGet("{rentalId}/return-info")]
    public async Task<IActionResult> GetReturnInfo(int rentalId)
    {
        try
        {
            var rentalFilter = new RentalFilter { RentalId = rentalId };
            var rentals = await _unitOfWork.RentalsRepository.GetRentalsAsync(rentalFilter);

            if (!rentals.Any())
                return NotFound($"Rental with ID {rentalId} not found");

            var rental = rentals.First();
            var latestReturn = rental.Returns.OrderByDescending(r => r.CreatedAt).FirstOrDefault();

            if (latestReturn == null)
                return NotFound($"No return found for rental {rentalId}");

            return Ok(ReturnMapper.ToDto(latestReturn));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching return information for rental {RentalId}", rentalId);
            return StatusCode(500, "An error occurred while fetching return information");
        }
    }
}