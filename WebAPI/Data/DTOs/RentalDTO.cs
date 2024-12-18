namespace WebAPI.Data.DTOs;

public sealed class RentalDto
{
    public int RentalId { get; set; }
    public int OfferId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public OfferDto Offer { get; set; } = null!;
}
