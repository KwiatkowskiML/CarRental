using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class RentalMapper
{
    public static RentalDto ToDto(Rental r)
    {
        return new RentalDto{
            RentalId = r.RentalId,
            OfferId = r.OfferId,
            RentalStatus = r.RentalStatus,
            CreatedAt = r.CreatedAt,
            Offer = OfferMapper.ToDto(r.Offer)
        };
    }
}