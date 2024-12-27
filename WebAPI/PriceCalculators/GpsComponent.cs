namespace WebAPI.PriceCalculators;

public class GpsComponent(decimal gpsDailyCost) : BaseComponent
{
    public override decimal CalculatePrice(PriceContext priceContext)
    {
        var price = priceContext.BasePrice;
        if (priceContext.Request.HasGps)
        {
            price += gpsDailyCost * priceContext.NumberOfDays;
        }
        return base.CalculatePrice(new PriceContext 
        { 
            BasePrice = price, 
            NumberOfDays = priceContext.NumberOfDays, 
            Request = priceContext.Request 
        });
    }
}