using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class OfferMapper
{
    public static OfferDto ToDto(Offer offer)
    {
        var offerDto = new OfferDto{
            OfferId = offer.OfferId,
            TotalPrice = offer.TotalPrice,
            CustomerId = offer.CustomerId,
            StartDate = offer.StartDate,
            EndDate = offer.EndDate,
            CreatedAt = offer.CreatedAt,
            HasGps = offer.HasGps,
            HasChildSeat = offer.HasChildSeat,
            Insurance = offer.Insurance,
            Car = CarMapper.ToDto(offer.Car!) // get rid of nullable fields
        };

        return offerDto;
    }
}