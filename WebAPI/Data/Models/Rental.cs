using System;
using System.Collections.Generic;
using WebAPI.Data.Models;

namespace CarRental.WebAPI.Data.Models;

public partial class Rental
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

    public InsuranceTypeEnum? InsuranceType { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Return> Returns { get; set; } = new List<Return>();
}
