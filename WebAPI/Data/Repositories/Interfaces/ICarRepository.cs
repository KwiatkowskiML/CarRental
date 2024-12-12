using CarRental.WebAPI.Data.DTOs;
using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Repositories.Interfaces;

public interface ICarRepository
{
    // consider car or carDto
    Task<List<Car>> GetCarsAsync(CarFilter filter);
}