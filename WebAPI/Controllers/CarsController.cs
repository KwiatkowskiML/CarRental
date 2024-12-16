using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Mappers;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.DTOs;
using WebAPI.filters;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCars([FromQuery] CarFilter filter)
    {
        try
        {
            var cars = await unitOfWork.CarsRepository.GetCarsAsync(filter);
            var carDtos = cars.Select(CarMapper.ToDto).ToList();
            return Ok(carDtos);
        }
        catch (DatabaseOperationException ex)
        {
            unitOfWork.LogError(ex, "Error fetching cars");
            return StatusCode(500, "An error occurred while fetching cars");
        }
    }
}