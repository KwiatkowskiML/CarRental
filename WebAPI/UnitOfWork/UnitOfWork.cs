using WebAPI.Data.Context;
using WebAPI.Data.Repositories;
using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.UnitOfWork;

public class UnitOfWork(CarRentalContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly CarRentalContext _context = context;
    private ILogger Logger { get; } = logger;

    public ICarRepository CarsRepository { get; } = new CarRepository(context, logger);
    public IUserRepository UsersRepository { get; } = new UserRepository(context, logger);
    public IRentalRepository RentalsRepository { get; } = new RentalRepository(context, logger);
    
    public IOfferRepository OffersRepository { get; } = new OfferRepository(context, logger);

    public void LogError(Exception ex, string message)
    {
        Logger.LogError(ex, message);
    }
    
    public void LogInformation(string message)
    {
        Logger.LogInformation(message);
    }

    public void SaveChanges() 
    {
        // Commit database changes
    }
}