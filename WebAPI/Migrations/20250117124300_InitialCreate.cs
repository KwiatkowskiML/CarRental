using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "car_providers",
                columns: table => new
                {
                    car_provider_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    api_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    contact_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    contact_phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("car_providers_pkey", x => x.car_provider_id);
                });

            migrationBuilder.CreateTable(
                name: "insurances",
                columns: table => new
                {
                    insurance_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("insurances_pkey", x => x.insurance_id);
                });

            migrationBuilder.CreateTable(
                name: "rental_status",
                columns: table => new
                {
                    rental_status_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rental_status_pkey", x => x.rental_status_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    age = table.Column<int>(type: "integer", nullable: false),
                    location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    car_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    car_provider_id = table.Column<int>(type: "integer", nullable: false),
                    license_plate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    engine_capacity = table.Column<decimal>(type: "numeric(3,1)", precision: 3, scale: 1, nullable: false),
                    power = table.Column<int>(type: "integer", nullable: false),
                    fuel_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    base_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    images = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cars_pkey", x => x.car_id);
                    table.ForeignKey(
                        name: "cars_car_provider_id_fkey",
                        column: x => x.car_provider_id,
                        principalTable: "car_providers",
                        principalColumn: "car_provider_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    customer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    driving_license_years = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("customers_pkey", x => x.customer_id);
                    table.ForeignKey(
                        name: "FK_customers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employees_pkey", x => x.employee_id);
                    table.ForeignKey(
                        name: "employees_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "offers",
                columns: table => new
                {
                    offer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    total_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    car_id = table.Column<int>(type: "integer", nullable: false),
                    insurance_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    has_gps = table.Column<bool>(type: "boolean", nullable: false),
                    has_child_seat = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("offers_pkey", x => x.offer_id);
                    table.ForeignKey(
                        name: "FK_offers_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "offers_car_id_fkey",
                        column: x => x.car_id,
                        principalTable: "cars",
                        principalColumn: "car_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "offers_insurance_id_fkey",
                        column: x => x.insurance_id,
                        principalTable: "insurances",
                        principalColumn: "insurance_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rentals",
                columns: table => new
                {
                    rental_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    offer_id = table.Column<int>(type: "integer", nullable: false),
                    rental_status_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("rentals_pkey", x => x.rental_id);
                    table.ForeignKey(
                        name: "rentals_offer_id_fkey",
                        column: x => x.offer_id,
                        principalTable: "offers",
                        principalColumn: "offer_id");
                    table.ForeignKey(
                        name: "rentals_rental_status_id_fkey",
                        column: x => x.rental_status_id,
                        principalTable: "rental_status",
                        principalColumn: "rental_status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "returns",
                columns: table => new
                {
                    return_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rental_id = table.Column<int>(type: "integer", nullable: true),
                    return_date = table.Column<DateOnly>(type: "date", nullable: false),
                    condition_description = table.Column<string>(type: "text", nullable: true),
                    photo_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    processed_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("returns_pkey", x => x.return_id);
                    table.ForeignKey(
                        name: "returns_rental_id_fkey",
                        column: x => x.rental_id,
                        principalTable: "rentals",
                        principalColumn: "rental_id");
                });

            migrationBuilder.CreateIndex(
                name: "car_providers_api_key_key",
                table: "car_providers",
                column: "api_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cars_car_provider_id",
                table: "cars",
                column: "car_provider_id");

            migrationBuilder.CreateIndex(
                name: "cars_license_plate_key",
                table: "cars",
                column: "license_plate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_user_id",
                table: "customers",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_user_id",
                table: "employees",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_offers_car_id",
                table: "offers",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_offers_customer_id",
                table: "offers",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_offers_insurance_id",
                table: "offers",
                column: "insurance_id");

            migrationBuilder.CreateIndex(
                name: "IX_rentals_offer_id",
                table: "rentals",
                column: "offer_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rentals_rental_status_id",
                table: "rentals",
                column: "rental_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_returns_rental_id",
                table: "returns",
                column: "rental_id");

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "returns");

            migrationBuilder.DropTable(
                name: "rentals");

            migrationBuilder.DropTable(
                name: "offers");

            migrationBuilder.DropTable(
                name: "rental_status");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "insurances");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "car_providers");
        }
    }
}
