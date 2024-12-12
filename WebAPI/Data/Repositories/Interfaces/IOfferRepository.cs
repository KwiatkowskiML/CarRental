using WebAPI.filters;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IOfferRepository
{
    Task<OfferDTO?> GetOffersAsync(OfferFilter filter);
}