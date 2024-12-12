using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Repositories;
using WebAPI.Data.Repositories.Interfaces;

namespace CarRental.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController2 : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CarsController2(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCars([FromQuery] CarFilter filter)
    {
        try
        {
            var cars = await _unitOfWork.CarsRepository.GetCarsAsync(filter);
                
            var carDTOs = cars.Select(c => new CarDTO
            {
                CarId = c.CarId,
                LicensePlate = c.LicensePlate,
                Brand = c.Brand,
                Model = c.Model,
                Year = c.Year,
                Status = c.Status,
                Location = c.Location,
                EngineCapacity = c.EngineCapacity,
                Power = c.Power,
                FuelType = c.FuelType,
                Description = c.Description,
                CarProvider = c.CarProvider != null ? new CarProviderDTO
                {
                    CarProviderId = c.CarProvider.CarProviderId,
                    Name = c.CarProvider.Name,
                    ContactEmail = c.CarProvider.ContactEmail,
                    ContactPhone = c.CarProvider.ContactPhone
                } : null
            }).ToList();

            return Ok(carDTOs);
        }
        catch (DatabaseOperationException ex)
        {
            _unitOfWork.LogError(ex, "Error fetching cars");
            return StatusCode(500, "An error occurred while fetching cars");
        }
    }
}