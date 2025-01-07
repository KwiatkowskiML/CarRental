using WebAPI.Requests;

namespace WebAPI.PriceCalculators;

public interface IPriceCalculator
{
    public decimal CalculatePrice(decimal carPrice, decimal insurancePrice, 
        int drivingYears, GetOfferRequest request);
}