using CarRental.WebAPI.Data.Models;
using WebAPI.DTOs;
using WebAPI.filters;

namespace WebAPI.Data.Repositories.Interfaces;

public interface ICarRepository
{
    // consider car or carDto
    Task<List<Car>> GetCarsAsync(CarFilter filter);
    Task<Car?> GetCarByIdAsync(int carId);
}