namespace WebAPI.PriceCalculators;

public class ChildSeatComponent(decimal childSeatDailyCost) : BaseComponent
{
    public override decimal CalculatePrice(PriceContext priceContext)
    {
        var price = priceContext.BasePrice;
        if (priceContext.Request.HasChildSeat)
        {
            price += childSeatDailyCost * priceContext.NumberOfDays;
        }
        return base.CalculatePrice(new PriceContext 
        { 
            BasePrice = price, 
            NumberOfDays = priceContext.NumberOfDays, 
            Request = priceContext.Request 
        });
    }
}