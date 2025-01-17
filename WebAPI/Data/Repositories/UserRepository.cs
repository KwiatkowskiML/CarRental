using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;

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
    
    public async Task<List<User>> GetUsersAsync(UserFilter filter)
    {
        try
        {
            IQueryable<User> query = Context.Users;
            
            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(u => u.Email.Contains(filter.Email));
            
            if (!string.IsNullOrEmpty(filter.FirstName))
                query = query.Where(u => u.FirstName.Contains(filter.FirstName));
            
            if (!string.IsNullOrEmpty(filter.LastName))
                query = query.Where(u => u.LastName.Contains(filter.LastName));
            
            if (filter.UserId.HasValue)
                query = query.Where(u => u.UserId == filter.UserId!.Value);
            
            if (filter.BirthDate.HasValue)
                query = query.Where(u => u.BirthDate == filter.BirthDate!.Value);
            
            return await query.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching users with filters");
            throw new DatabaseOperationException("Failed to fetch users", ex);
        }
    }

    public async Task<Customer?> GetCustomerByUserIdAsync(int userId)
    {
        try
        {
            return await Context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching customer by userId");
            throw new DatabaseOperationException($"Failed to fetch customer with userId {userId}", ex);
        }
    }

    public async Task<Customer?> GetCustomerAsync(int customerId)
    {
        try
        {
            return await Context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching customer by customerId");
            throw new DatabaseOperationException($"Failed to fetch customer with customerId {customerId}", ex);
        }
    }

    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            var existingUser = await Context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
                throw new InvalidOperationException("A user with this email already exists.");
            
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
            var userExists = await Context.Users.AnyAsync(u => u.UserId == customer.UserId);

            if (!userExists)
                throw new InvalidOperationException($"User with ID {customer.UserId} does not exist.");

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