using WebAPI.Data.Models;

namespace WebAPI.Services;

public interface IExternalCarProvider
{
    string Name { get; }
    Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime start, DateTime end);
}