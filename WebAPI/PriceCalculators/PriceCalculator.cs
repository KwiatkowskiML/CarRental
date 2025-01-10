using WebAPI.Requests;

namespace WebAPI.PriceCalculators;

public class PriceCalculator: IPriceCalculator
{
    private readonly IPriceCalculatorComponent _chain;

    public PriceCalculator(decimal gpsDailyCost = 10.0m, decimal childSeatDailyCost = 15.00m)
    {
        var gpsComponent = new GpsComponent(gpsDailyCost);
        var childSeatComponent = new ChildSeatComponent(childSeatDailyCost);
        
        gpsComponent.SetNextComponent(childSeatComponent);
        _chain = gpsComponent;
    }
    
    public decimal CalculatePrice(decimal carPrice, decimal insurancePrice, int drivingYears, GetOfferRequest request)
    {
        if (drivingYears < 0)
            throw new ArgumentException("Driving years cannot be negative");
        
        int numberOfDays = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;
        decimal basePrice = carPrice * numberOfDays;
        decimal additionalTax = drivingYears > 0 ? basePrice / drivingYears : basePrice;
        basePrice += additionalTax + insurancePrice * numberOfDays;

        var context = new PriceContext
        {
            BasePrice = basePrice,
            NumberOfDays = numberOfDays,
            Request = request
        };

        return _chain.CalculatePrice(context);
    }
}