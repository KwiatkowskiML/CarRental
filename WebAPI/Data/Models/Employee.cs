using System;
using System.Collections.Generic;

namespace CarRental.WebAPI.Data.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int? UserId { get; set; }

    public string Role { get; set; } = null!;

    public virtual User? User { get; set; }
}
