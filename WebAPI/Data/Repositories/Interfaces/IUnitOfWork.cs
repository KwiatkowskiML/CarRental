namespace WebAPI.Data.Repositories.Interfaces;

public interface IUnitOfWork
{
    ICarRepository CarsRepository { get; }
    IUserRepository UsersRepository { get; }
    IRentalRepository RentalsRepository { get; }
    void LogError(Exception ex, string message);
    void SaveChanges();
}