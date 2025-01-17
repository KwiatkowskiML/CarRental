using WebAPI.Data.Models;

namespace WebAPI.Data.DTOs;

public sealed class OfferDto
{
    public int OfferId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool? HasGps { get; set; }
    public bool? HasChildSeat { get; set; }
    public Insurance? Insurance { get; set; }
    public CarDto? Car { get; set; }
    public CustomerDto? Customer { get; set; }
}