namespace WebAPI.Data.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    public int? UserId { get; set; }

    public string Role { get; set; } = null!;

    public virtual User? User { get; set; }
}
