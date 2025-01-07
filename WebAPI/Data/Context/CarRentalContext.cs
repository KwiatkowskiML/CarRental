using Microsoft.EntityFrameworkCore;
using WebAPI.Data.Models;

namespace WebAPI.Data.Context;

public partial class CarRentalContext(DbContextOptions<CarRentalContext> options) : DbContext(options)
{
    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarProvider> CarProviders { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<Return> Returns { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<Insurance> Insurances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("cars_pkey");

            entity.ToTable("cars");

            entity.HasIndex(e => e.LicensePlate, "cars_license_plate_key").IsUnique();

            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.CarProviderId).HasColumnName("car_provider_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EngineCapacity)
                .HasPrecision(3, 1)
                .HasColumnName("engine_capacity");
            entity.Property(e => e.FuelType)
                .HasMaxLength(50)
                .HasColumnName("fuel_type");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.Power).HasColumnName("power");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.BasePrice)
                .HasColumnName("base_price")
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Images)
                .HasColumnName("images")
                .HasColumnType("text[]");
            entity.HasOne(d => d.CarProvider).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarProviderId)
                .HasConstraintName("cars_car_provider_id_fkey");
            entity.HasMany(c => c.Offers)
                .WithOne(o => o.Car)
                .HasForeignKey(o => o.CarId)
                .HasConstraintName("offers_car_id_fkey");
        });

        modelBuilder.Entity<CarProvider>(entity =>
        {
            entity.HasKey(e => e.CarProviderId).HasName("car_providers_pkey");

            entity.ToTable("car_providers");

            entity.HasIndex(e => e.ApiKey, "car_providers_api_key_key").IsUnique();

            entity.Property(e => e.CarProviderId).HasColumnName("car_provider_id");
            entity.Property(e => e.ApiKey)
                .HasMaxLength(255)
                .HasColumnName("api_key");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(255)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(50)
                .HasColumnName("contact_phone");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DrivingLicenseYears).HasColumnName("driving_license_years");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            
            entity.HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .IsRequired() 
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Role)
                .HasMaxLength(100)
                .HasColumnName("role");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Employees)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("employees_user_id_fkey");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.RentalId).HasName("rentals_pkey");

            entity.ToTable("rentals");

            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.OfferId).HasColumnName("offer_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.RentalStatusId).HasColumnName("rental_status_id");
            
            entity.HasOne(d => d.RentalStatus).WithMany()
                .HasForeignKey(d => d.RentalStatusId)
                .HasConstraintName("rentals_rental_status_id_fkey");

            entity.HasOne(d => d.Offer)
                .WithOne(p => p.Rental)
                .HasForeignKey<Rental>(d => d.OfferId)
                .HasConstraintName("rentals_offer_id_fkey")
                .IsRequired(true);
        });

        modelBuilder.Entity<Return>(entity =>
        {
            entity.HasKey(e => e.ReturnId).HasName("returns_pkey");

            entity.ToTable("returns");

            entity.Property(e => e.ReturnId).HasColumnName("return_id");
            entity.Property(e => e.ConditionDescription).HasColumnName("condition_description");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .HasColumnName("photo_url");
            entity.Property(e => e.ProcessedBy).HasColumnName("processed_by");
            entity.Property(e => e.RentalId).HasColumnName("rental_id");
            entity.Property(e => e.ReturnDate).HasColumnName("return_date");

            entity.HasOne(d => d.Rental).WithMany(p => p.Returns)
                .HasForeignKey(d => d.RentalId)
                .HasConstraintName("returns_rental_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("offers_pkey");

            entity.ToTable("offers");

            entity.Property(e => e.TotalPrice)
                .HasColumnName("total_price")
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.OfferId).HasColumnName("offer_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.InsuranceId).HasColumnName("insurance_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.HasChildSeat).HasColumnName("has_child_seat");
            entity.Property(e => e.HasGps).HasColumnName("has_gps");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Insurance).WithMany()
                .HasForeignKey(d => d.InsuranceId)
                .HasConstraintName("offers_insurance_id_fkey");

            entity.HasOne(d => d.Rental)
                .WithOne(p => p.Offer)
                .HasForeignKey<Rental>(d => d.OfferId)
                .IsRequired(false);
        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.HasKey(e => e.InsuranceId).HasName("insurances_pkey");

            entity.ToTable("insurances");

            entity.Property(e => e.InsuranceId).HasColumnName("insurance_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.name).HasColumnName("name");
        });
        
        modelBuilder.Entity<RentalStatus>(entity =>
        {
            entity.HasKey(e => e.RentalStatusId).HasName("rental_status_pkey");

            entity.ToTable("rental_status");

            entity.Property(e => e.RentalStatusId).HasColumnName("rental_status_id");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
