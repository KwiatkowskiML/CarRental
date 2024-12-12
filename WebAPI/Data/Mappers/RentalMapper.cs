using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Mappers;

public static class RentalMapper
{
    public static RentalDTO ToDto(Rental r)
    {
        return new RentalDTO{
            RentalId = r.RentalId,
            OfferId = r.OfferId,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            Offer = OfferMapper.ToDto(r.Offer)
        };
    }
}