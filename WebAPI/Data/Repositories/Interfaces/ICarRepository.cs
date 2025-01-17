using WebAPI.Data.Models;
using WebAPI.filters;

namespace WebAPI.Data.Repositories.Interfaces;

public interface ICarRepository
{
    Task<(List<Car> Cars, int TotalCount)> GetPaginatedCarsAsync(CarFilter filter, int page, int pageSize);
    Task<List<Car>> GetCarsAsync(CarFilter filter);
    Task<Car?> GetCarByIdAsync(int carId);
}