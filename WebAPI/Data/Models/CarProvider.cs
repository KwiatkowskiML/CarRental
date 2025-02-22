﻿namespace WebAPI.Data.Models;

public class CarProvider
{
    public int CarProviderId { get; set; }

    public string Name { get; set; } = null!;

    public string ApiKey { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
