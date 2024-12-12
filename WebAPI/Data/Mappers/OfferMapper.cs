using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Mappers
{
    public class OfferMapper: IMapper<Offer, OfferDTO>
    {
        private static readonly CarMapper CarMapper = new CarMapper();
        public OfferDTO ToDto(Offer offer)
        {
            var offerDto = new OfferDTO{
                OfferId = offer.OfferId,
                TotalPrice = offer.TotalPrice,
                CustomerId = offer.CustomerId,
                StartDate = offer.StartDate,
                EndDate = offer.EndDate,
                CreatedAt = offer.CreatedAt,
                HasGps = offer.HasGps,
                HasChildSeat = offer.HasChildSeat,
                Insurance = offer.Insurance,
                Car = CarMapper.ToDto(offer.Car)
            };

            return offerDto;
        }
    }
}