using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Context;
using WebAPI.Data.Models;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.Exceptions;
using WebAPI.filters;

namespace WebAPI.Data.Repositories;

public class CustomerRepository: BaseRepository<Customer>, ICustomerRepository
{
    private ILogger<CustomerRepository> _logger;
    public CustomerRepository(CarRentalContext context, ILogger<CustomerRepository> logger) : base(context, logger)
    {
        _logger = logger;
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

    public async Task<List<Customer>> GetCustomersAsync(CustomerFilter filter)
    {
        try
        {
            IQueryable<Customer> query = Context.Customers.Include(c => c.User);
            
            if (filter.UserId.HasValue)
                query = query.Where(c => c.UserId == filter.UserId.Value);
            
            if (filter.DrivingLicenseYears.HasValue)
                query = query.Where(c => c.DrivingLicenseYears == filter.DrivingLicenseYears.Value);
            
            if (filter.CustomerId.HasValue)
                query = query.Where(c => c.CustomerId == filter.CustomerId.Value);
            
            return await query.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching customers with filters");
            throw new DatabaseOperationException("Failed to fetch customers", ex);
        }
    }
}