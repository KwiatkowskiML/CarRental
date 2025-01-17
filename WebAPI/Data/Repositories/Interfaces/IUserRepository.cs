using WebAPI.Data.Models;
using WebAPI.filters;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IUserRepository 
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<Customer?> GetCustomerByUserIdAsync(int userId);
    Task<Customer> CreateCustomerAsync(Customer customer);
    Task<Customer?> GetCustomerAsync(int customerId);
    Task<User> CreateUserAsync(User user);
    Task<List<User>> GetUsersAsync(UserFilter filter);
}