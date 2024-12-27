namespace WebAPI.PriceCalculators;

public interface IPriceCalculatorComponent
{
    decimal CalculatePrice(PriceContext context);
    void SetNextComponent(IPriceCalculatorComponent? component);
}