namespace WebAPI.PriceCalculators;

public class BaseComponent: IPriceCalculatorComponent
{
    protected IPriceCalculatorComponent? NextComponent;
    
    public void SetNextComponent(IPriceCalculatorComponent? nextComponent)
    {
        NextComponent = nextComponent;
    }

    public virtual decimal CalculatePrice(PriceContext context)
    {
        return NextComponent?.CalculatePrice(context) ?? context.BasePrice;
    }
}