using System;
using System.Collections.Generic;

namespace CarRental.WebAPI.Data.Models;

public partial class RentalDTO
{
    public int RentalId { get; set; }

    public int? CustomerId { get; set; }

    public int? CarId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? HasGps { get; set; }

    public bool? HasChildSeat { get; set; }

    public int InsuranceId { get; set; }

    public virtual Insurance? Insurance { get; set; }
}
