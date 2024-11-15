using System;
using System.Collections.Generic;

namespace CarRental.WebAPI.Data.Models;

public partial class Return
{
    public int ReturnId { get; set; }

    public int? RentalId { get; set; }

    public DateOnly ReturnDate { get; set; }

    public string? ConditionDescription { get; set; }

    public string? PhotoUrl { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Rental? Rental { get; set; }
}
