# CarRental API Documentation

## Project Overview
A car rental management system that handles car inventory, rentals, and returns. The system supports multiple car providers and manages the entire rental lifecycle.

## Project Structure
```
CarRental.WebAPI/
├── Controllers/
│   └── CarsController.cs
├── Data/
│   ├── Context/
│   │   └── CarRentalContext.cs
│   ├── Models/
│   │   ├── Car.cs
│   │   ├── CarProvider.cs
│   │   ├── Customer.cs
│   │   ├── Employee.cs
│   │   ├── Rental.cs
│   │   ├── Return.cs
│   │   └── User.cs
│   ├── DTOs/
│   │   └── CarFilter.cs
│   └── Repositories/
│       ├── Interfaces/
│       │   └── ICarRentalRepository.cs
│       └── CarRentalRepository.cs
└── Exceptions/
    └── DatabaseOperationException.cs
```

## Data Models

### User
Base user entity with personal information:
```csharp
public class User
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Customer
Extends User with rental-specific information:
```csharp
public class Customer
{
    public int CustomerId { get; set; }
    public int UserId { get; set; }
    public int DrivingLicenseYears { get; set; }
}
```

### Car
Represents a rental vehicle:
```csharp
public class Car
{
    public int CarId { get; set; }
    public int CarProviderId { get; set; }
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Status { get; set; }  // available, rented, maintenance
    public string? Location { get; set; }
    public decimal EngineCapacity { get; set; }
    public int Power { get; set; }
    public string FuelType { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## Repository Pattern

### ICarRentalRepository Interface
Defines the contract for database operations:
```csharp
public interface ICarRentalRepository
{
    Task<List<Car>> GetAllCarsAsync();
    Task<List<Car>> GetAvailableCarsAsync(CarFilter filter);
    Task<Car?> GetCarByIdAsync(int carId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<List<Rental>> GetUserRentalsAsync(int userId);
    Task<bool> CreateRentalAsync(Rental rental);
    Task<bool> ProcessReturnAsync(Return carReturn);
}
```

## Adding New Features

### 1. Create a New Model
```csharp
public class NewFeature
{
    public int Id { get; set; }
    // Add properties
    public DateTime CreatedAt { get; set; }
}
```

### 2. Add DbSet to Context
```csharp
public class CarRentalContext : DbContext
{
    public DbSet<NewFeature> NewFeatures { get; set; }
}
```

### 3. Create Repository Interface
```csharp
public interface INewFeatureRepository
{
    Task<NewFeature> GetByIdAsync(int id);
    Task<bool> CreateAsync(NewFeature feature);
}
```

### 4. Implement Repository
```csharp
public class NewFeatureRepository : INewFeatureRepository
{
    private readonly CarRentalContext _context;
    private readonly ILogger<NewFeatureRepository> _logger;

    public NewFeatureRepository(
        CarRentalContext context,
        ILogger<NewFeatureRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Implement interface methods
}
```

## Common Patterns

### Error Handling
Use the DatabaseOperationException for database-related errors:
```csharp
try
{
    // Database operation
}
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed");
    throw new DatabaseOperationException("Failed to perform operation", ex);
}
```

### Repository Methods
Follow this pattern for repository methods:
1. Use async/await
2. Include related entities with Include()
3. Use transactions for multiple operations
4. Handle errors consistently

Example:
```csharp
public async Task<List<T>> GetItemsAsync()
{
    try
    {
        return await _context.Items
            .Include(i => i.RelatedEntity)
            .AsNoTracking()
            .ToListAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching items");
        throw new DatabaseOperationException("Failed to fetch items", ex);
    }
}
```

### Filtering
Use the CarFilter pattern for complex queries:
```csharp
public class NewFilter
{
    public string? SearchTerm { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
```

## Configuration

### Database Connection
In appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=car_rental;Username=root;Password=my_password;SSL Mode=Disable"
  }
}
```

### Dependency Injection
In Program.cs:
```csharp
builder.Services.AddDbContext<CarRentalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICarRentalRepository, CarRentalRepository>();
```

## Status Values

### Car Status
- "available": Car is ready for rental
- "rented": Car is currently rented
- "maintenance": Car is under maintenance

### Rental Status
- "active": Ongoing rental
- "completed": Rental finished
- "cancelled": Rental was cancelled

## Best Practices

1. **Async Operations**
   - Always use async/await for database operations
   - Use cancellation tokens for long-running operations

2. **Data Access**
   - Use Include() to load related entities
   - Use AsNoTracking() for read-only queries
   - Use transactions for multiple operations

3. **Error Handling**
   - Log errors with appropriate level
   - Use custom exceptions
   - Include meaningful error messages

4. **Performance**
   - Use pagination for large datasets
   - Implement caching where appropriate
   - Monitor query performance

5. **Security**
   - Validate input data
   - Use parameterized queries
   - Implement proper authorization

## Adding New Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class NewController : ControllerBase
{
    private readonly INewFeatureRepository _repository;
    private readonly ILogger<NewController> _logger;

    public NewController(
        INewFeatureRepository repository,
        ILogger<NewController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] NewFilter filter)
    {
        try
        {
            var results = await _repository.GetItemsAsync(filter);
            return Ok(results);
        }
        catch (DatabaseOperationException ex)
        {
            _logger.LogError(ex, "Error in Get operation");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}
```