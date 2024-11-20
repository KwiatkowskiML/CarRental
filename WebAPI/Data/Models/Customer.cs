using System;
using System.Collections.Generic;

namespace CarRental.WebAPI.Data.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int? UserId { get; set; }

    public int DrivingLicenseYears { get; set; }

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
