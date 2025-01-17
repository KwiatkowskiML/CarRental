﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebAPI.Data.Context;

#nullable disable

namespace WebAPI.Migrations
{
    [DbContext(typeof(CarRentalContext))]
    [Migration("20250117134851_ChangeAgeColumnToBirthDate")]
    partial class ChangeAgeColumnToBirthDate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WebAPI.Data.Models.Car", b =>
                {
                    b.Property<int>("CarId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("car_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CarId"));

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("base_price");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("brand");

                    b.Property<int>("CarProviderId")
                        .HasColumnType("integer")
                        .HasColumnName("car_provider_id");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<decimal>("EngineCapacity")
                        .HasPrecision(3, 1)
                        .HasColumnType("numeric(3,1)")
                        .HasColumnName("engine_capacity");

                    b.Property<string>("FuelType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("fuel_type");

                    b.Property<string[]>("Images")
                        .HasColumnType("text[]")
                        .HasColumnName("images");

                    b.Property<string>("LicensePlate")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("license_plate");

                    b.Property<string>("Location")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("location");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("model");

                    b.Property<int>("Power")
                        .HasColumnType("integer")
                        .HasColumnName("power");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("status");

                    b.Property<int>("Year")
                        .HasColumnType("integer")
                        .HasColumnName("year");

                    b.HasKey("CarId")
                        .HasName("cars_pkey");

                    b.HasIndex("CarProviderId");

                    b.HasIndex(new[] { "LicensePlate" }, "cars_license_plate_key")
                        .IsUnique();

                    b.ToTable("cars", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.CarProvider", b =>
                {
                    b.Property<int>("CarProviderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("car_provider_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CarProviderId"));

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("api_key");

                    b.Property<string>("ContactEmail")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("contact_email");

                    b.Property<string>("ContactPhone")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("contact_phone");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.HasKey("CarProviderId")
                        .HasName("car_providers_pkey");

                    b.HasIndex(new[] { "ApiKey" }, "car_providers_api_key_key")
                        .IsUnique();

                    b.ToTable("car_providers", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("customer_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CustomerId"));

                    b.Property<int>("DrivingLicenseYears")
                        .HasColumnType("integer")
                        .HasColumnName("driving_license_years");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("CustomerId")
                        .HasName("customers_pkey");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("customers", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("employee_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EmployeeId"));

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("role");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("EmployeeId")
                        .HasName("employees_pkey");

                    b.HasIndex("UserId");

                    b.ToTable("employees", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.Insurance", b =>
                {
                    b.Property<int>("InsuranceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("insurance_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("InsuranceId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("price");

                    b.HasKey("InsuranceId")
                        .HasName("insurances_pkey");

                    b.ToTable("insurances", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.Offer", b =>
                {
                    b.Property<int>("OfferId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("offer_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OfferId"));

                    b.Property<int>("CarId")
                        .HasColumnType("integer")
                        .HasColumnName("car_id");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer")
                        .HasColumnName("customer_id");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date")
                        .HasColumnName("end_date");

                    b.Property<bool>("HasChildSeat")
                        .HasColumnType("boolean")
                        .HasColumnName("has_child_seat");

                    b.Property<bool>("HasGps")
                        .HasColumnType("boolean")
                        .HasColumnName("has_gps");

                    b.Property<int>("InsuranceId")
                        .HasColumnType("integer")
                        .HasColumnName("insurance_id");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date")
                        .HasColumnName("start_date");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("total_price");

                    b.HasKey("OfferId")
                        .HasName("offers_pkey");

                    b.HasIndex("CarId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("InsuranceId");

                    b.ToTable("offers", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.Rental", b =>
                {
                    b.Property<int>("RentalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("rental_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RentalId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("OfferId")
                        .HasColumnType("integer")
                        .HasColumnName("offer_id");

                    b.Property<int>("RentalStatusId")
                        .HasColumnType("integer")
                        .HasColumnName("rental_status_id");

                    b.HasKey("RentalId")
                        .HasName("rentals_pkey");

                    b.HasIndex("OfferId")
                        .IsUnique();

                    b.HasIndex("RentalStatusId");

                    b.ToTable("rentals", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.RentalStatus", b =>
                {
                    b.Property<int>("RentalStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("rental_status_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RentalStatusId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.HasKey("RentalStatusId")
                        .HasName("rental_status_pkey");

                    b.ToTable("rental_status", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.Return", b =>
                {
                    b.Property<int>("ReturnId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("return_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ReturnId"));

                    b.Property<string>("ConditionDescription")
                        .HasColumnType("text")
                        .HasColumnName("condition_description");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("PhotoUrl")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("photo_url");

                    b.Property<int?>("ProcessedBy")
                        .HasColumnType("integer")
                        .HasColumnName("processed_by");

                    b.Property<int?>("RentalId")
                        .HasColumnType("integer")
                        .HasColumnName("rental_id");

                    b.Property<DateOnly>("ReturnDate")
                        .HasColumnType("date")
                        .HasColumnName("return_date");

                    b.HasKey("ReturnId")
                        .HasName("returns_pkey");

                    b.HasIndex("RentalId");

                    b.ToTable("returns", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<DateOnly>("BirthDate")
                        .HasColumnType("date")
                        .HasColumnName("age");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("last_name");

                    b.Property<string>("Location")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("location");

                    b.HasKey("UserId")
                        .HasName("users_pkey");

                    b.HasIndex(new[] { "Email" }, "users_email_key")
                        .IsUnique();

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("WebAPI.Data.Models.Car", b =>
                {
                    b.HasOne("WebAPI.Data.Models.CarProvider", "CarProvider")
                        .WithMany("Cars")
                        .HasForeignKey("CarProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("cars_car_provider_id_fkey");

                    b.Navigation("CarProvider");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Customer", b =>
                {
                    b.HasOne("WebAPI.Data.Models.User", "User")
                        .WithOne()
                        .HasForeignKey("WebAPI.Data.Models.Customer", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Employee", b =>
                {
                    b.HasOne("WebAPI.Data.Models.User", "User")
                        .WithMany("Employees")
                        .HasForeignKey("UserId")
                        .HasConstraintName("employees_user_id_fkey");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Offer", b =>
                {
                    b.HasOne("WebAPI.Data.Models.Car", "Car")
                        .WithMany("Offers")
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("offers_car_id_fkey");

                    b.HasOne("WebAPI.Data.Models.Customer", "Customer")
                        .WithMany("Offers")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebAPI.Data.Models.Insurance", "Insurance")
                        .WithMany()
                        .HasForeignKey("InsuranceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("offers_insurance_id_fkey");

                    b.Navigation("Car");

                    b.Navigation("Customer");

                    b.Navigation("Insurance");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Rental", b =>
                {
                    b.HasOne("WebAPI.Data.Models.Offer", "Offer")
                        .WithOne("Rental")
                        .HasForeignKey("WebAPI.Data.Models.Rental", "OfferId")
                        .HasConstraintName("rentals_offer_id_fkey");

                    b.HasOne("WebAPI.Data.Models.RentalStatus", "RentalStatus")
                        .WithMany()
                        .HasForeignKey("RentalStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("rentals_rental_status_id_fkey");

                    b.Navigation("Offer");

                    b.Navigation("RentalStatus");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Return", b =>
                {
                    b.HasOne("WebAPI.Data.Models.Rental", "Rental")
                        .WithMany("Returns")
                        .HasForeignKey("RentalId")
                        .HasConstraintName("returns_rental_id_fkey");

                    b.Navigation("Rental");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Car", b =>
                {
                    b.Navigation("Offers");
                });

            modelBuilder.Entity("WebAPI.Data.Models.CarProvider", b =>
                {
                    b.Navigation("Cars");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Customer", b =>
                {
                    b.Navigation("Offers");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Offer", b =>
                {
                    b.Navigation("Rental");
                });

            modelBuilder.Entity("WebAPI.Data.Models.Rental", b =>
                {
                    b.Navigation("Returns");
                });

            modelBuilder.Entity("WebAPI.Data.Models.User", b =>
                {
                    b.Navigation("Employees");
                });
#pragma warning restore 612, 618
        }
    }
}
