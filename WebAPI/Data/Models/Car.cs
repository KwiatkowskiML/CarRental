﻿namespace WebAPI.Data.Models;

public class Car
{
    public int CarId { get; set; }
    public int CarProviderId { get; set; }
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
    public DateTime? CreatedAt { get; set; }
    public decimal BasePrice { get; set; }
    public string[]? Images { get; set; }

    public virtual CarProvider? CarProvider { get; set; }
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
