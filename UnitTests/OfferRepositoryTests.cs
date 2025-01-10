using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebAPI.Data.Models;
using WebAPI.Data.Context;
using WebAPI.filters;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Data.Repositories;
using WebAPI.Exceptions;

public class OfferRepositoryTests : IDisposable
{
    private readonly CarRentalContext _context;
    private readonly ILogger<OfferRepository> _logger;
    private readonly OfferRepository _repository;

    public OfferRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new CarRentalContext(options);
        _logger = Mock.Of<ILogger<OfferRepository>>();
        _repository = new OfferRepository(_context, _logger);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            UserId = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var customer = new Customer { CustomerId = 1, User = user };
        var carProvider = new CarProvider
        {
            CarProviderId = 1,
            Name = "Test Provider",
            ApiKey = "test-api-key",
            ContactEmail = "test@provider.com"
        };
        var car = new Car
        {
            CarId = 1,
            Brand = "Toyota",
            Model = "Camry",
            Year = 2020,
            Location = "New York",
            FuelType = "Gasoline",
            Status = "available",
            LicensePlate = "ABC123",
            CarProvider = carProvider
        };
        var insurance = new Insurance
        {
            InsuranceId = 1,
            Name = "Basic Insurance",
            Price = 10.0m
        };

        _context.Users.Add(user);
        _context.Customers.Add(customer);
        _context.Cars.Add(car);
        _context.Insurances.Add(insurance);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Should_Insert_Offer_Correctly()
    {
        // Arrange
        var offer = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            HasGps = true,
            HasChildSeat = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _repository.CreateOfferAsync(offer);

        // Assert
        var savedOffer = await _repository.GetOfferAsync(new OfferFilter { OfferId = offer.OfferId });
        Assert.NotNull(savedOffer);
    }

    [Fact]
    public async Task Should_Validate_Date_Ranges()
    {
        // Arrange
        var offer = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), // Invalid date range
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseOperationException>(() => 
            _repository.CreateOfferAsync(offer));
    }

    [Fact]
    public async Task Should_Check_Car_Availability()
    {
        // Arrange
        // First offer
        var offer1 = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateOfferAsync(offer1);

        // Add a rental for the first offer
        var rental = new Rental
        {
            OfferId = offer1.OfferId,
            RentalStatusId = 1 // Active rental
        };
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        // Second offer with overlapping dates
        var offer2 = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseOperationException>(() => 
            _repository.CreateOfferAsync(offer2));
    }

    [Fact]
    public async Task Should_Include_All_Required_Relationships()
    {
        // Arrange
        var offer = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateOfferAsync(offer);

        // Act
        var savedOffer = await _repository.GetOfferAsync(new OfferFilter { OfferId = offer.OfferId });

        // Assert
        Assert.NotNull(savedOffer);
        Assert.NotNull(savedOffer.Car);
        Assert.NotNull(savedOffer.Car.CarProvider);
        Assert.NotNull(savedOffer.Customer);
        Assert.NotNull(savedOffer.Customer.User);
        Assert.NotNull(savedOffer.Insurance);
    }

    [Fact]
    public async Task Should_Handle_Invalid_Insurance_Types()
    {
        // Arrange
        var offer = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 999, // Non-existent insurance
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseOperationException>(() => 
            _repository.CreateOfferAsync(offer));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeletedAsync().Wait();
        _context.Dispose();
    }
}