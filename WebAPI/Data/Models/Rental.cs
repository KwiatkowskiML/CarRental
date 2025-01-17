namespace WebAPI.Data.Models;

public class Rental
{
    public int RentalId { get; set; }
    public int OfferId { get; set; }
    public int RentalStatusId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public virtual Offer Offer { get; set; } = null!;
    public virtual RentalStatus RentalStatus { get; set; } = null!;
    public virtual ICollection<Return> Returns { get; set; } = new List<Return>();
}