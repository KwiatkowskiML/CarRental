using WebAPI.Data.Context;
using WebAPI.Data.Repositories;
using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly CarRentalContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    
    public UnitOfWork(CarRentalContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger<UnitOfWork>();
        
        // Create repositories with their specific loggers
        CarsRepository = new CarRepository(context, loggerFactory.CreateLogger<CarRepository>());
        UsersRepository = new UserRepository(context, loggerFactory.CreateLogger<UserRepository>());
        RentalsRepository = new RentalRepository(context, loggerFactory.CreateLogger<RentalRepository>());
        OffersRepository = new OfferRepository(context, loggerFactory.CreateLogger<OfferRepository>());
    }

    public ICarRepository CarsRepository { get; }
    public IUserRepository UsersRepository { get; }
    public IRentalRepository RentalsRepository { get; }
    public IOfferRepository OffersRepository { get; }

    public void LogError(Exception ex, string message)
    {
        _logger.LogError(ex, message);
    }
    
    public void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    public void SaveChanges() 
    {
        // Commit database changes
    }
}