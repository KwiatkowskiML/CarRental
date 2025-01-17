using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.DTOs;

namespace WebAPI.Services;

public class SuperRentalCarProvider : IExternalCarProvider
{
    private readonly HttpClient _httpClient;
    private readonly CarRentalContext _context;
    private readonly ILogger<SuperRentalCarProvider> _logger;
    private CarProvider? _provider;
    
    public string Name => "SuperRental";

    public SuperRentalCarProvider(
        HttpClient httpClient,
        CarRentalContext context,
        ILogger<SuperRentalCarProvider> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri("http://rentalapi-esauh2huedhcc2a6.polandcentral-01.azurewebsites.net");
    }

    private async Task EnsureProviderInitialized()
    {
        if (_provider == null)
        {
            _provider = await _context.CarProviders
                .FirstOrDefaultAsync(cp => cp.Name == Name);

            if (_provider == null)
            {
                _logger.LogError("SuperRental provider not found in database");
                throw new InvalidOperationException("SuperRental provider not found in database");
            }
        }
    }

    public async Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime start, DateTime end)
    {
        try
        {
            await EnsureProviderInitialized();

            var queryParams = new Dictionary<string, string>
            {
                { "start", start.ToString("O") },
                { "end", end.ToString("O") }
            };

            var query = string.Join("&", queryParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            var response = await _httpClient.GetFromJsonAsync<List<SuperRentalVehicleDto>>($"/api/Vehicle/available?{query}");

            if (response == null)
                return Enumerable.Empty<Car>();

            return response.Select(v => new Car
            {
                CarProviderId = _provider!.CarProviderId,
                CarProvider = _provider,
                Brand = v.Brand,
                Model = v.Model,
                Year = v.YearOfProduction,
                Status = "available",
                Location = v.ToolLocation,
                Description = v.Description,
                LicensePlate = v.RegisterNo,
                BasePrice = v.Price,
                Images = v.PhotoUrl != null ? new[] { v.PhotoUrl } : Array.Empty<string>(),
                FuelType = "Unknown", // Could be mapped from v.DriveType if that contains fuel info
                Power = 0, // Not provided in external API
                EngineCapacity = 0 // Not provided in external API
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cars from SuperRental");
            return Enumerable.Empty<Car>();
        }
    }
}