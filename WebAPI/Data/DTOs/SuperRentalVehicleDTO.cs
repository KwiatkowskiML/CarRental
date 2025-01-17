namespace WebAPI.Data.DTOs;

public class SuperRentalVehicleDto
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int YearOfProduction { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string DriveType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string ToolLocation { get; set; } = string.Empty;
    public string SerialNo { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string RegisterNo { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
}