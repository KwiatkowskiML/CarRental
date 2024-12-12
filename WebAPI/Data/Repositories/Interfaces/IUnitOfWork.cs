using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.Data.Repositories;

public interface IUnitOfWork
{
    ICarRepository CarsRepository { get; }
    void LogError(Exception ex, string message);
    void SaveChanges();
}