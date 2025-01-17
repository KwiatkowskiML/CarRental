using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Storage.V1;
using WebAPI.Constants;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;
using WebAPI.Mappers;
using WebAPI.Requests;
using WebAPI.Services.Interfaces;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkerController(IUnitOfWork unitOfWork, IEmailService emailService, ILogger<WorkerController> logger) : ControllerBase
    {
        [HttpGet("rentals")]
        public async Task<IActionResult> GetUserRentals([FromQuery] RentalFilter request)
        {
            try
            {
                var rentals = await unitOfWork.RentalsRepository.GetRentalsAsync(request);
                var rentalDtos = rentals.Select(RentalMapper.ToDto).ToList();
                return Ok(rentalDtos);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }

        [HttpPost("accept-return")]
        public async Task<IActionResult> AcceptReturn([FromBody] AcceptReturnRequest request)
        {
            try
            {
                var rentalFilter = new RentalFilter() { RentalId = request.RentalId };
                var rentals = await unitOfWork.RentalsRepository.GetRentalsAsync(rentalFilter);

                if (rentals.Count == 0)
                    return NotFound($"Rental with RentalId {request.RentalId} not found");

                var rental = rentals.First();
                if (rental.RentalStatusId != RentalStatus.GetPendingId())
                    return BadRequest("Return is not pending");

                var completedReturn = await unitOfWork.RentalsRepository.ProcessReturn(request);
                var returnDto = ReturnMapper.ToDto(completedReturn);

                var rentalDto = RentalMapper.ToDto(rental);
                var email = rentalDto.Offer.Customer!.UserDto.Email;
                var name = rentalDto.Offer.Customer!.UserDto.FirstName;
                await emailService.SendReturnCompletionInvoiceEmail(email, name, rentalDto);
                
                return Ok(returnDto);
            }
            catch (DatabaseOperationException ex)
            {
                unitOfWork.LogError(ex, "Error fetching rentals");
                return StatusCode(500, "An error occurred while fetching rentals");
            }
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                // Validate file type
                if (!file.ContentType.StartsWith("image/"))
                    return BadRequest("Invalid file type");

                // Validate file size (5MB)
                if (file.Length > 100 * 1024 * 1024)
                    return BadRequest("File size exceeds limit");

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                // Upload to Google Cloud Storage
                using var stream = file.OpenReadStream();
                var storageClient = await StorageClient.CreateAsync();
                var obj = await storageClient.UploadObjectAsync(StorageConstants.BucketName, fileName, 
                    file.ContentType, stream);

                // Generate public URL
                var publicUrl = $"{StorageConstants.PublicUrlPrefix}/{StorageConstants.BucketName}/{fileName}";

                return Ok(new { url = publicUrl });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading file");
                return StatusCode(500, "Failed to upload file");
            }
        }
    }
}