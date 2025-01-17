using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;
using WebAPI.Mappers;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CarProviderAggregator _providerAggregator;

    public CarsController(
        IUnitOfWork unitOfWork,
        CarProviderAggregator providerAggregator)
    {
        _unitOfWork = unitOfWork;
        _providerAggregator = providerAggregator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCars([FromQuery] CarFilter filter)
    {
        try
        {
            // Get internal cars
            var ownCars = await _unitOfWork.CarsRepository.GetCarsAsync(filter);
            var ownCarDtos = ownCars.Select(CarMapper.ToDto).ToList();

            // Get external cars
            var externalCars = await _providerAggregator.GetAllAvailableCarsAsync(
                filter.StartDate ?? DateTime.MinValue,
                filter.EndDate ?? DateTime.MaxValue
            );

            // Apply filters to external cars
            var filteredExternalCars = externalCars.AsQueryable();
            
            if (!string.IsNullOrEmpty(filter.Location))
                filteredExternalCars = filteredExternalCars.Where(c => c.Location == filter.Location);

            if (!string.IsNullOrEmpty(filter.Brand))
                filteredExternalCars = filteredExternalCars.Where(c => 
                    c.Brand.ToLower().Contains(filter.Brand.ToLower()));

            if (!string.IsNullOrEmpty(filter.Model))
                filteredExternalCars = filteredExternalCars.Where(c => 
                    c.Model.ToLower().Contains(filter.Model.ToLower()));

            if (filter.MinYear.HasValue)
                filteredExternalCars = filteredExternalCars.Where(c => c.Year >= filter.MinYear.Value);

            if (filter.MaxYear.HasValue)
                filteredExternalCars = filteredExternalCars.Where(c => c.Year <= filter.MaxYear.Value);

            if (!string.IsNullOrEmpty(filter.FuelType))
                filteredExternalCars = filteredExternalCars.Where(c => c.FuelType == filter.FuelType);

            // Convert external cars to DTOs and combine with internal cars
            var externalCarDtos = filteredExternalCars.Select(CarMapper.ToDto);
            ownCarDtos.AddRange(externalCarDtos);

            return Ok(ownCarDtos);
        }
        catch (DatabaseOperationException ex)
        {
            _unitOfWork.LogError(ex, "Error fetching cars");
            return StatusCode(500, "An error occurred while fetching cars");
        }
        catch (Exception ex)
        {
            _unitOfWork.LogError(ex, "Unexpected error while fetching cars");
            return StatusCode(500, "An unexpected error occurred while fetching cars");
        }
    }
}