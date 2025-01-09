using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebAPI.Data.Models;
using WebAPI.Data.Context;
using WebAPI.filters;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Data.Repositories;
using WebAPI.Exceptions;
using WebAPI.Requests;

public class RentalRepositoryTests : IDisposable
{
    private readonly CarRentalContext _context;
    private readonly ILogger<RentalRepository> _logger;
    private readonly RentalRepository _repository;

    public RentalRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new CarRentalContext(options);
        _logger = Mock.Of<ILogger<RentalRepository>>();
        _repository = new RentalRepository(_context, _logger);

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

        // Add test offer
        var offer = new Offer
        {
            OfferId = 1,
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            CreatedAt = DateTime.UtcNow
        };
        _context.Offers.Add(offer);

        var rentalStatus = new RentalStatus
        {
            RentalStatusId = 1,
            Description = "Confirmed"
        };
        _context.RentalStatuses.Add(rentalStatus);
        
        // Add test rental
        var rental = new Rental
        {
            RentalId = 1,
            OfferId = 1,
            RentalStatusId = 1,
            CreatedAt = DateTime.UtcNow
        };
        _context.Rentals.Add(rental);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetRentalsAsync_WithValidFilter_ShouldReturnRentals()
    {
        // Arrange
        var filter = new RentalFilter { CustomerId = 1 };

        // Act
        var rentals = await _repository.GetRentalsAsync(filter);

        // Assert
        Assert.NotEmpty(rentals);
        Assert.All(rentals, r => Assert.Equal(1, r.Offer.CustomerId));
    }

    [Fact]
    public async Task GetRentalsAsync_WithNullFilter_ShouldReturnAllRentals()
    {
        // Act
        var rentals = await _repository.GetRentalsAsync(null);

        // Assert
        Assert.NotEmpty(rentals);
    }

    [Fact]
    public async Task GetRentalsAsync_WithInvalidFilter_ShouldReturnEmptyList()
    {
        // Arrange
        var filter = new RentalFilter { CustomerId = 999 };

        // Act
        var rentals = await _repository.GetRentalsAsync(filter);

        // Assert
        Assert.Empty(rentals);
    }

    [Fact]
    public async Task CreateRentalFromOffer_WithValidOffer_ShouldCreateRental()
    {
        // Arrange
        var newOffer = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(13)),
            CreatedAt = DateTime.UtcNow
        };
        _context.Offers.Add(newOffer);
        await _context.SaveChangesAsync();

        // Act
        var rental = await _repository.CreateRentalFromOfferAsync(newOffer.OfferId);

        // Assert
        Assert.NotNull(rental);
        Assert.Equal(newOffer.OfferId, rental.OfferId);
    }

    [Fact]
    public async Task CreateRentalFromOffer_WithNonexistentOffer_ShouldReturnNull()
    {
        // Act
        var rental = await _repository.CreateRentalFromOfferAsync(999);

        // Assert
        Assert.Null(rental);
    }

    [Fact]
    public async Task CreateRentalFromOffer_WithOverlappingDates_ShouldThrowException()
    {
        // Arrange
        var overlappingOffer = new Offer
        {
            CarId = 1,
            CustomerId = 1,
            InsuranceId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)),
            CreatedAt = DateTime.UtcNow
        };
        _context.Offers.Add(overlappingOffer);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseOperationException>(() =>
            _repository.CreateRentalFromOfferAsync(overlappingOffer.OfferId));
    }

    [Fact]
    public async Task GetRentalByOfferId_WithValidOfferId_ShouldReturnRental()
    {
        // Arrange
        int offerId = 1;
        
        // Act
        var rental = await _repository.GetRentalByOfferIdAsync(offerId);

        // Assert
        Assert.NotNull(rental);
        Assert.Equal(offerId, rental.OfferId);
    }

    [Fact]
    public async Task GetRentalByOfferId_WithInvalidOfferId_ShouldReturnNull()
    {
        // Arrange
        int invalidOfferId = 999;
        
        // Act
        var rental = await _repository.GetRentalByOfferIdAsync(invalidOfferId);

        // Assert
        Assert.Null(rental);
    }

    [Fact]
    public async Task InitReturn_WithValidRentalId_ShouldSetStatusToPending()
    {
        // Arrange
        int rentalId = 1;
        
        // Act
        var result = await _repository.InitReturn(rentalId);

        // Assert
        Assert.True(result);
        var rental = await _context.Rentals.FindAsync(rentalId);
        Assert.NotNull(rental);
        Assert.Equal(RentalStatus.GetPendingId(), rental.RentalStatusId);
    }

    [Fact]
    public async Task ProcessReturn_WithValidRequest_ShouldCreateReturnAndSetStatus()
    {
        // Arrange
        int rentalId = 1;
        var request = new AcceptReturnRequest
        {
            RentalId = rentalId,
            ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ConditionDescription = "Good condition",
            PhotoUrl = "http://example.com/photo.jpg",
            EmployeeId = 1
        };
        
        // Act
        var result = await _repository.ProcessReturn(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(rentalId, result.RentalId);
        var rental = await _context.Rentals.FindAsync(rentalId);
        Assert.NotNull(rental);
        Assert.Equal(RentalStatus.GetCompletedId(), rental.RentalStatusId);
    }

    [Fact]
    public async Task ProcessReturn_WithInvalidRentalId_ShouldThrowException()
    {
        // Arrange
        int invalidRentalId = 999;
        var request = new AcceptReturnRequest
        {
            RentalId = invalidRentalId,
            ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ConditionDescription = "Good condition",
            PhotoUrl = "http://example.com/photo.jpg",
            EmployeeId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseOperationException>(() =>
            _repository.ProcessReturn(request));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeletedAsync().Wait();
        _context.Dispose();
    }
}