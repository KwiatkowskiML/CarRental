using WebAPI.DTOs;

namespace WebAPI.HelperClasses;

public static class PriceCalculator
{
    private const decimal GpsDailyCost = 10.00m;
    private const decimal ChildSeatDailyCost = 15.00m;
    
    public static decimal GetPrice(decimal carPrice, decimal insurancePrice, 
        decimal drivingYears, GetOfferRequest request)
    {
        int numberOfDays = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;
        decimal totalPrice = carPrice * numberOfDays;
        totalPrice += totalPrice / drivingYears;

        totalPrice += insurancePrice * numberOfDays;

        if (request.HasGps)
            totalPrice += GpsDailyCost * numberOfDays;
        if (request.HasChildSeat)
            totalPrice += ChildSeatDailyCost * numberOfDays;

        return totalPrice;
    }
}