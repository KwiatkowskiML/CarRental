using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebAPI.Data.Models;
using WebAPI.Data.Context;
using WebAPI.filters;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Data.Repositories;
using WebAPI.Exceptions;

public class UserRepositoryTests : IDisposable
{
    private readonly CarRentalContext _context;
    private readonly ILogger<UserRepository> _logger;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CarRentalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new CarRentalContext(options);
        _logger = Mock.Of<ILogger<UserRepository>>();
        _repository = new UserRepository(_context, _logger);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var user = new User
        {
            UserId = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateOnly(1990, 1, 1),
            CreatedAt = DateTime.UtcNow
        };

        var customer = new Customer
        {
            CustomerId = 1,
            UserId = 1,
            User = user,
            DrivingLicenseYears = 5,
        };

        _context.Users.Add(user);
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserByEmail_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        String email = "test@example.com";
        
        // Act
        var user = await _repository.GetUserByEmailAsync(email);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public async Task GetUserByEmail_WithNonexistentEmail_ShouldReturnNull()
    {
        // Arrange
        String nonExistingEmail = "nonexistent@example.com";
        
        // Act
        var user = await _repository.GetUserByEmailAsync(nonExistingEmail);

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task GetUsers_WithEmailFilter_ShouldReturnMatchingUsers()
    {
        // Arrange
        String email = "test";
        var filter = new UserFilter { Email = email};

        // Act
        var users = await _repository.GetUsersAsync(filter);

        // Assert
        Assert.NotEmpty(users);
        Assert.All(users, u => Assert.Contains(email, u.Email));
    }

    [Fact]
    public async Task GetUsers_WithMultipleFilters_ShouldReturnMatchingUsers()
    {
        // Arrange
        String name = "John";
        String lastName = "Doe";
        DateOnly birthDate = new DateOnly(1990, 1, 1);
        
        var filter = new UserFilter
        {
            FirstName = name,
            LastName = lastName,
            BirthDate = birthDate
        };

        // Act
        var users = await _repository.GetUsersAsync(filter);

        // Assert
        Assert.NotEmpty(users);
        Assert.All(users, u => 
        {
            Assert.Contains(name, u.FirstName);
            Assert.Contains(lastName, u.LastName);
            Assert.Equal(birthDate, u.BirthDate);
        });
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var email = "new@example.com";
        var firstName = "Jane";
        var lastName = "Smith";
        var birthDate = new DateOnly(1990, 1, 1);
        var createdAt = DateTime.UtcNow;

        var newUser = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            CreatedAt = createdAt
        };

        // Act
        var createdUser = await _repository.CreateUserAsync(newUser);

        // Assert
        Assert.NotNull(createdUser);
        Assert.NotEqual(0, createdUser.UserId);
        Assert.Equal(email, createdUser.Email);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var duplicateUser = new User
        {
            Email = "test@example.com", // Already exists in seed data
            FirstName = "Jane",
            LastName = "Smith",
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseOperationException>(() => 
            _repository.CreateUserAsync(duplicateUser));
    }

    [Fact]
    public async Task GetCustomerByUserId_WithValidId_ShouldReturnCustomer()
    {
        // Arrange
        var userId = 1;

        // Act
        var customer = await _repository.GetCustomerByUserIdAsync(userId);

        // Assert
        Assert.NotNull(customer);
        Assert.NotNull(customer.User);
        Assert.Equal(userId, customer.UserId);
        Assert.Equal(5, customer.DrivingLicenseYears);
    }
    
    [Fact]
    public async Task GetCustomerByUserId_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidUserId = 999;

        // Act
        var customer = await _repository.GetCustomerByUserIdAsync(invalidUserId);

        // Assert
        Assert.Null(customer);
    }

    [Fact]
    public async Task GetCustomer_WithValidId_ShouldReturnCustomer()
    {
        // Arrange
        var customerId = 1;

        // Act
        var customer = await _repository.GetCustomerAsync(customerId);

        // Assert
        Assert.NotNull(customer);
        Assert.NotNull(customer.User);
        Assert.Equal(customerId, customer.CustomerId);
    }

    [Fact]
    public async Task CreateCustomer_WithValidData_ShouldCreateCustomer()
    {
        // Arrange
        var email = "newcustomer@example.com";
        var firstName = "Jane";
        var lastName = "Smith";
        var createdAt = DateTime.UtcNow;
        var drivingLicenseYears = 3;

        var newUser = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = createdAt
        };
        await _repository.CreateUserAsync(newUser);

        var newCustomer = new Customer
        {
            UserId = newUser.UserId,
            DrivingLicenseYears = drivingLicenseYears
        };

        // Act
        var createdCustomer = await _repository.CreateCustomerAsync(newCustomer);

        // Assert
        Assert.NotNull(createdCustomer);
        Assert.Equal(drivingLicenseYears, createdCustomer.DrivingLicenseYears);
        Assert.Equal(newUser.UserId, createdCustomer.UserId);
    }

    [Fact]
    public async Task GetUsers_WithNoMatchingFilters_ShouldReturnEmptyList()
    {
        // Arrange
        var filter = new UserFilter
        {
            FirstName = "NonExistent",
            LastName = "User"
        };

        // Act
        var users = await _repository.GetUsersAsync(filter);

        // Assert
        Assert.Empty(users);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var invalidCustomer = new Customer
        {
            UserId = 999, // Non-existent user
            DrivingLicenseYears = 3
        };

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseOperationException>(() => 
            _repository.CreateCustomerAsync(invalidCustomer));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeletedAsync().Wait();
        _context.Dispose();
    }
}