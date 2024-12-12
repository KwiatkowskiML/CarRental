using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Mappers;

public class RentalMapper: IMapper<Rental, RentalDTO>
{
    private static readonly OfferMapper OfferMapper = new OfferMapper();
    public RentalDTO ToDto(Rental r)
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