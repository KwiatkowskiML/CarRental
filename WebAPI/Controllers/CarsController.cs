using Microsoft.AspNetCore.Mvc;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;
using WebAPI.Mappers;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController(IUnitOfWork unitOfWork) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCars([FromQuery] CarFilter filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        try
        {
            var (cars, totalCount) = await unitOfWork.CarsRepository.GetPaginatedCarsAsync(filter, page, pageSize);
            var carDtos = cars.Select(CarMapper.ToDto).ToList();

            return Ok(new
            {
                cars = carDtos,
                totalCount = totalCount,
                currentPage = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }
        catch (DatabaseOperationException ex)
        {
            unitOfWork.LogError(ex, "Error fetching cars");
            return StatusCode(500, "An error occurred while fetching cars");
        }
    }
}