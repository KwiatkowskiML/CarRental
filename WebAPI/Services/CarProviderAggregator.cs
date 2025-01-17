using WebAPI.Data.Models;
namespace WebAPI.Services;

public class CarProviderAggregator
{
    private readonly IEnumerable<IExternalCarProvider> _providers;
    private readonly ILogger<CarProviderAggregator> _logger;

    public CarProviderAggregator(
        IEnumerable<IExternalCarProvider> providers,
        ILogger<CarProviderAggregator> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task<IEnumerable<Car>> GetAllAvailableCarsAsync(DateTime start, DateTime end)
    {
        var tasks = _providers.Select(p => GetProviderCarsAsync(p, start, end));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(cars => cars);
    }

    private async Task<IEnumerable<Car>> GetProviderCarsAsync(IExternalCarProvider provider, DateTime start, DateTime end)
    {
        try
        {
            var cars = await provider.GetAvailableCarsAsync(start, end);
            return cars;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cars from provider {ProviderName}", provider.Name);
            return Enumerable.Empty<Car>();
        }
    }
}