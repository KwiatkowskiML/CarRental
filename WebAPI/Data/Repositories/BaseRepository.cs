using CarRental.WebAPI.Data.Context;

namespace WebAPI.Data.Repositories;

public abstract class BaseRepository<TEntity>(CarRentalContext context, ILogger logger)
    where TEntity : class
{
    protected readonly CarRentalContext Context = context;
    protected readonly ILogger Logger = logger;
}