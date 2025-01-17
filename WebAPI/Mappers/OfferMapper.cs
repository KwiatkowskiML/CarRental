using WebAPI.Data.DTOs;
using WebAPI.Data.Models;

namespace WebAPI.Mappers;

public static class OfferMapper
{
    public static OfferDto ToDto(Offer offer)
    {
        if (offer.Customer == null)
            throw new ArgumentException("Offer must have a Customer", nameof(offer));
            
        if (offer.Customer.User == null)
            throw new ArgumentException("Customer must have a User", nameof(offer));
            
        if (offer.Car == null)
            throw new ArgumentException("Offer must have a Car", nameof(offer));

        var offerDto = new OfferDto{
            OfferId = offer.OfferId,
            TotalPrice = offer.TotalPrice,
            StartDate = offer.StartDate,
            EndDate = offer.EndDate,
            CreatedAt = offer.CreatedAt,
            HasGps = offer.HasGps,
            HasChildSeat = offer.HasChildSeat,
            Insurance = offer.Insurance,
            Customer = CustomerMapper.ToDto(offer.Customer!),
            Car = CarMapper.ToDto(offer.Car!) // get rid of nullable fields
        };

        return offerDto;
    }
}