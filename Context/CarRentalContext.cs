using Microsoft.EntityFrameworkCore;
using CarRental.WebAPI.Data.Entities;

namespace CarRental.WebAPI.Data.Context
{
    public class CarRentalContext : DbContext
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarProvider> CarProviders { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Return> Returns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure entities here...
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.UserId);
                // Add other configurations...
            });

            // Configure other entities...
        }
    }
}