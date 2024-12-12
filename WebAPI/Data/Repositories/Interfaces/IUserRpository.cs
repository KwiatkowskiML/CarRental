using CarRental.WebAPI.Data.Models;

namespace WebAPI.Data.Repositories.Interfaces;

public interface IUserRepository 
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<Customer?> GetCustomerByUserIdAsync(int userId);
    Task<Customer> CreateCustomerAsync(Customer customer);
}