namespace WebAPI.Data.Repositories.Interfaces;

public interface IUnitOfWork
{
    ICarRepository CarsRepository { get; }
    IUserRepository UsersRepository { get; }
    IRentalRepository RentalsRepository { get; }
    IOfferRepository OffersRepository { get; }
    ICustomerRepository CustomersRepository { get; }
    
    void LogError(Exception ex, string message);
    void LogInformation(string message);
    void SaveChanges();
}