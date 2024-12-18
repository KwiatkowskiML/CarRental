using WebAPI.Data.Models;
using WebAPI.filters;

namespace WebAPI.Data.Repositories.Interfaces;

public interface ICarRepository
{
    Task<List<Car>> GetCarsAsync(CarFilter filter);
    Task<Car?> GetCarByIdAsync(int carId);
}