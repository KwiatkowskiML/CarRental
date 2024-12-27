using WebAPI.Requests;

namespace WebAPI.PriceCalculators;

public class PriceCalculator(decimal gpsDailyCost = 10.0m, decimal childSeatDailyCost = 15.00m)
    : IPriceCalculator
{
    public decimal CalculatePrice(decimal carPrice, decimal insurancePrice, int drivingYears, GetOfferRequest request)
    {
        int numberOfDays = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;
        decimal totalPrice = carPrice * numberOfDays;

        decimal additionalTax = totalPrice;
        if (drivingYears > 0)
            additionalTax /= drivingYears;

        totalPrice += additionalTax + insurancePrice * numberOfDays;

        if (request.HasGps)
            totalPrice += gpsDailyCost * numberOfDays;
        if (request.HasChildSeat)
            totalPrice += childSeatDailyCost * numberOfDays;

        return totalPrice;
    }
}