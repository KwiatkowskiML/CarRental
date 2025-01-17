using WebAPI.Data.Models;

namespace WebAPI.Data.DTOs;

public sealed class RentalDto
{
    public int RentalId { get; set; }
    public int OfferId { get; set; }
    public required RentalStatus RentalStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public OfferDto Offer { get; set; } = null!;
}
