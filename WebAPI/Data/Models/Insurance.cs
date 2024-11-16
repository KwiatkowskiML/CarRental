using System;
using System.Collections.Generic;

namespace CarRental.WebAPI.Data.Models;

public partial class Insurance
{
    public int InsuranceId { get; set; }

    public decimal Price { get; set; }

    public string name { get; set; } = null!;
}
