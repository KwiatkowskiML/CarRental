namespace WebAPI.Data.Models;

public class Customer
{
    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public int DrivingLicenseYears { get; set; }
    
    public virtual User User { get; set; }

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();
}
