using System;
using System.Collections.Generic;

namespace CarRental.WebAPI.Data.Models;

public partial class RentalDTO
{
    public int RentalId { get; set; }
    public int OfferId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public virtual OfferDTO Offer { get; set; } = null!;
}
