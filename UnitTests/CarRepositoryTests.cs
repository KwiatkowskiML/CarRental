using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories;
using WebAPI.Exceptions;
using WebAPI.filters;
using Xunit.Abstractions;

namespace UnitTests;

public class CarRepositoryTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly CarRentalContext _context;
    private readonly CarRepository _repository;
    private readonly ILogger<CarRepository> _logger;

    public CarRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CarRentalContext(options);
        _logger = Mock.Of<ILogger<CarRepository>>();
        _repository = new CarRepository(_context, _logger);

        SeedTestData();
    }


    private void SeedTestData()
    {
        var carProvider = new CarProvider
        {
            CarProviderId = 1,
            Name = "Test Provider",
            ApiKey = "test-api-key",
            ContactEmail = "test@provider.com"
        };

        var cars = new List<Car>
        {
            new()
            {
                CarId = 1,
                Brand = "Toyota",
                Model = "Camry",
                Year = 2020,
                Location = "New York",
                FuelType = "Gasoline",
                Status = "available",
                LicensePlate = "ABC123",
                CarProvider = carProvider,
                Offers = new List<Offer>()
            },
            new()
            {
                CarId = 2,
                Brand = "Honda",
                Model = "Civic",
                Year = 2019,
                Location = "Los Angeles",
                FuelType = "Hybrid",
                Status = "available",
                LicensePlate = "DEF456",
                CarProvider = carProvider,
                Offers = new List<Offer>
                {
                    new()
                    {
                        OfferId = 1,
                        StartDate = DateOnly.FromDateTime(DateTime.Today),
                        EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
                        Rental = new Rental { RentalStatusId = 1 } // Active rental
                    }
                }
            },
            new()
            {
                CarId = 3,
                Brand = "Tesla",
                Model = "Model 3",
                Year = 2022,
                Location = "Miami",
                FuelType = "Electric",
                Status = "available",
                LicensePlate = "GHI789",
                CarProvider = carProvider,
                Offers = new List<Offer>()
            }
        };

        _context.CarProviders.Add(carProvider);
        _context.Cars.AddRange(cars);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Should_Filter_Available_Cars_Correctly()
    {
        // Arrange
        var filter = new CarFilter();

        // Act
        var result = await _repository.GetCarsAsync(filter);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task Should_Filter_By_Date_Range_Correctly()
    {
        // Arrange
        var filter = new CarFilter
        {
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(3)
        };

        // Act
        var result = await _repository.GetCarsAsync(filter);

        // Assert
        Assert.Equal(2, result.Count); // Only car without overlapping rental
        Assert.All(result, car => Assert.DoesNotContain(car.CarId, new[] {2}));
    }

    [Fact]
    public async Task Should_Filter_By_Location_Correctly()
    {
        // Arrange
        var filter = new CarFilter
        {
            Location = "New York"
        };

        // Act
        var result = await _repository.GetCarsAsync(filter);

        // Assert
        Assert.Single(result);
        Assert.All(result, car => Assert.Equal("New York", car.Location));
    }

    [Fact]
    public async Task Should_Filter_By_Multiple_Criteria_Simultaneously()
    {
        // Arrange
        var filter = new CarFilter
        {
            Location = "New York",
            Brand = "Toyota",
            MinYear = 2020,
            FuelType = "Gasoline"
        };

        // Act
        var result = await _repository.GetCarsAsync(filter);

        // Assert
        Assert.Single(result);
        var car = result[0];
        Assert.Equal("New York", car.Location);
        Assert.Equal("Toyota", car.Brand);
        Assert.True(car.Year >= 2020);
        Assert.Equal("Gasoline", car.FuelType);
    }

    [Fact]
    public async Task Should_Handle_Invalid_Filter_Parameters()
    {
        // Arrange
        var filter = new CarFilter
        {
            Location = "NonExistentLocation",
            Brand = "NonExistentBrand"
        };

        // Act
        var result = await _repository.GetCarsAsync(filter);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Should_Exclude_Cars_With_Overlapping_Rentals()
    {
        // Arrange
        var filter = new CarFilter
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(2)
        };

        // Act
        var result = await _repository.GetCarsAsync(filter);

        // Assert
        Assert.DoesNotContain(result, car => car.CarId == 2); // Car with overlapping rental
    }

    [Fact]
    public async Task Should_Get_Car_By_Id_Successfully()
    {
        // Arrange
        var carId = 1;

        // Act
        var result = await _repository.GetCarByIdAsync(carId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(carId, result.CarId);
        Assert.NotNull(result.CarProvider);
    }

    [Fact]
    public async Task Should_Return_Null_For_NonExistent_Car_Id()
    {
        // Arrange
        var nonExistentCarId = 999;

        // Act
        var result = await _repository.GetCarByIdAsync(nonExistentCarId);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeletedAsync().Wait();
        _context.Dispose();
    }
}