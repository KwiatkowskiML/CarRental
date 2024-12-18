namespace WebAPI.Data.DTOs;

public class CarDto
{
    public int CarId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public string Status { get; set; } = null!;
    public string? Location { get; set; }
    public decimal EngineCapacity { get; set; }
    public int Power { get; set; }
    public string FuelType { get; set; } = null!;
    public string? Description { get; set; }
    public CarProviderDto? CarProvider { get; set; }
}