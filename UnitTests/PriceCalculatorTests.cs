using WebAPI.PriceCalculators;
using WebAPI.Requests;

namespace UnitTests;

public class PriceCalculatorTests
{
    private readonly decimal _defaultCarPrice = 100.0m;
    private readonly decimal _defaultInsurancePrice = 20.0m;
    private readonly decimal _defaultGpsCost = 10.0m;
    private readonly decimal _defaultChildSeatCost = 15.0m;

    private PriceCalculator CreateCalculator() => 
        new PriceCalculator(gpsDailyCost: _defaultGpsCost, childSeatDailyCost: _defaultChildSeatCost);

    private GetOfferRequest CreateDefaultRequest(DateOnly startDate, DateOnly endDate, bool includeGps = false, bool includeChildSeat = false) =>
        new GetOfferRequest
        {
            StartDate = startDate,
            EndDate = endDate,
            HasGps = includeGps,
            HasChildSeat = includeChildSeat
        };

    [Fact]
    public void Should_Calculate_Base_Price_Correctly_For_Standard_Rental()
    {
        // Arrange
        var calculator = CreateCalculator();
        var request = CreateDefaultRequest(
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 1)
        );
        int drivingYears = 5;

        // Act
        decimal price = calculator.CalculatePrice(_defaultCarPrice, _defaultInsurancePrice, drivingYears, request);

        // Assert
        decimal expectedBasePrice = _defaultCarPrice; // One day rental
        decimal expectedTax = expectedBasePrice / drivingYears;
        decimal expectedInsurance = _defaultInsurancePrice;
        decimal expected = expectedBasePrice + expectedTax + expectedInsurance;
        Assert.Equal(expected, price);
    }
    
    [Theory]
    [InlineData(5, 20)] // 5 years experience
    [InlineData(10, 10)] // 10 years experience
    public void Should_Apply_Driving_Experience_Factor_Correctly(int drivingYears, decimal expectedTax)
    {
        // Arrange
        var calculator = CreateCalculator();
        var request = CreateDefaultRequest(
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 1)
        );

        // Act
        decimal price = calculator.CalculatePrice(_defaultCarPrice, _defaultInsurancePrice, drivingYears, request);

        // Assert
        decimal expectedBasePrice = _defaultCarPrice;
        decimal expectedInsurance = _defaultInsurancePrice;
        decimal expected = expectedBasePrice + expectedTax + expectedInsurance;
        Assert.Equal(expected, price);
    }

    [Fact]
    public void Should_Add_GPS_Cost_When_Selected()
    {
        // Arrange
        var calculator = CreateCalculator();
        var request = CreateDefaultRequest(
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 2),
            includeGps: true
        );
        int drivingYears = 5;

        // Act
        decimal price = calculator.CalculatePrice(_defaultCarPrice, _defaultInsurancePrice, drivingYears, request);

        // Assert
        decimal expectedBasePrice = _defaultCarPrice * 2; // Two day rental
        decimal expectedTax = expectedBasePrice / drivingYears;
        decimal expectedInsurance = _defaultInsurancePrice * 2;
        decimal expectedGpsCost = _defaultGpsCost * 2;
        decimal expected = expectedBasePrice + expectedTax + expectedInsurance + expectedGpsCost;
        Assert.Equal(expected, price);
    }

    [Fact]
    public void Should_Add_Child_Seat_Cost_When_Selected()
    {
        // Arrange
        var calculator = CreateCalculator();
        var request = CreateDefaultRequest(
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 2),
            includeChildSeat: true
        );
        int drivingYears = 5;

        // Act
        decimal price = calculator.CalculatePrice(_defaultCarPrice, _defaultInsurancePrice, drivingYears, request);

        // Assert
        decimal expectedBasePrice = _defaultCarPrice * 2; // Two day rental
        decimal expectedTax = expectedBasePrice / drivingYears;
        decimal expectedInsurance = _defaultInsurancePrice * 2;
        decimal expectedChildSeatCost = _defaultChildSeatCost * 2;
        decimal expected = expectedBasePrice + expectedTax + expectedInsurance + expectedChildSeatCost;
        Assert.Equal(expected, price);
    }

    [Fact]
    public void Should_Calculate_Multi_Day_Rentals_Correctly()
    {
        // Arrange
        var calculator = CreateCalculator();
        var request = CreateDefaultRequest(
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 5)  // 5 day rental
        );
        int drivingYears = 5;

        // Act
        decimal price = calculator.CalculatePrice(_defaultCarPrice, _defaultInsurancePrice, drivingYears, request);

        // Assert
        decimal expectedBasePrice = _defaultCarPrice * 5;
        decimal expectedTax = expectedBasePrice / drivingYears;
        decimal expectedInsurance = _defaultInsurancePrice * 5;
        decimal expected = expectedBasePrice + expectedTax + expectedInsurance;
        Assert.Equal(expected, price);
    }

    [Fact]
    public void Should_Handle_Zero_Driving_Years_Correctly()
    {
        // Arrange
        var calculator = CreateCalculator();
        var request = CreateDefaultRequest(
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 1)
        );
        int drivingYears = 0;

        // Act
        decimal price = calculator.CalculatePrice(_defaultCarPrice, _defaultInsurancePrice, drivingYears, request);

        // Assert
        decimal expectedBasePrice = _defaultCarPrice;
        decimal expectedTax = expectedBasePrice; // Should charge full base price as tax
        decimal expectedInsurance = _defaultInsurancePrice;
        decimal expected = expectedBasePrice + expectedTax + expectedInsurance;
        Assert.Equal(expected, price);
    }

    [Fact]
    public void Should_Throw_Exception_For_Negative_Driving_Years()
    {
        // Arrange
        var calculator = CreateCalculator();
        var request = CreateDefaultRequest(
            new DateOnly(2024, 1, 1),
            new DateOnly(2024, 1, 1)
        );
        int drivingYears = -1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            calculator.CalculatePrice(_defaultCarPrice, _defaultInsurancePrice, drivingYears, request));
    }
}