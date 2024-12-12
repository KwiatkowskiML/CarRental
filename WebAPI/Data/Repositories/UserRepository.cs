using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;

namespace WebAPI.Data.Repositories;

public class UserRepository(CarRentalContext context, ILogger logger) : BaseRepository<User>(context, logger), IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<Customer?> GetCustomerByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        throw new NotImplementedException();
    }
}