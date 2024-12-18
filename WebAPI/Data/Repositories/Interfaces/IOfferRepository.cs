using WebAPI.Data.Models;
using WebAPI.filters;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IOfferRepository
{
    Task<Offer?> GetOfferAsync(OfferFilter filter);
    Task<Insurance?> GetInsuranceByIdAsync(int insuranceId);
    Task CreateOfferAsync(Offer offer);
}