using CarRental.WebAPI.Data.Context;
using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.Data.Repositories;

public class UnitOfWork(CarRentalContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly CarRentalContext _context = context;
    private ILogger Logger { get; } = logger;

    public ICarRepository CarsRepository { get; } = new CarRepository(context, logger);
    public IUserRepository UsersRepository { get; } = new UserRepository(context, logger);
    public IRentalRepository RentalsRepository { get; } = new RentalRepository(context, logger);

    public void LogError(Exception ex, string message)
    {
        Logger.LogError(ex, message);
    }

    public void SaveChanges() 
    {
        // Commit database changes
    }
}