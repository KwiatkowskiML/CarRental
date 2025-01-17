using WebAPI.Requests;

namespace WebAPI.PriceCalculators;

public class PriceContext
{
    public decimal BasePrice { get; set; }
    public int NumberOfDays { get; set; }
    public GetOfferRequest Request { get; set; }
}