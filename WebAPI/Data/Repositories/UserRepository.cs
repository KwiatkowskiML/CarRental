using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;

namespace WebAPI.Data.Repositories;

public class UserRepository(CarRentalContext context, ILogger logger) : BaseRepository<User>(context, logger), IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            return await Context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching user by email");
            throw new DatabaseOperationException($"Failed to fetch user with email {email}", ex);
        }
    }

    public async Task<Customer?> GetCustomerByUserIdAsync(int userId)
    {
        try
        {
            return await Context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching customer by userId");
            throw new DatabaseOperationException($"Failed to fetch customer with userId {userId}", ex);
        }
    }
    
    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            Context.Users.Add(user);
            await Context.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating user");
            throw new DatabaseOperationException("Failed to create user", ex);
        }
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        try
        {
            Context.Customers.Add(customer);
            await Context.SaveChangesAsync();
            return customer;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating customer");
            throw new DatabaseOperationException("Failed to create customer", ex);
        }
    }
}