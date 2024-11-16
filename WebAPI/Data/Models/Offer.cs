using System;
using System.Collections.Generic;

namespace CarRental.WebAPI.Data.Models;

public partial class Offer
{
    public int OfferId { get; set; }

    public decimal TotalPrice { get; set; }

    public int? CustomerId { get; set; }

    public int? CarId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? HasGps { get; set; }

    public bool? HasChildSeat { get; set; }

    public string? InsuranceType { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Customer? Customer { get; set; }
}
