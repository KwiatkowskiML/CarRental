using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAgeColumnToBirthDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the new column
            migrationBuilder.AddColumn<DateOnly>(
                name: "birth_date",
                table: "users",
                type: "date",
                nullable: false);
            
            migrationBuilder.Sql(
                "UPDATE users SET birth_date = NOW() - INTERVAL '1 year' * age");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "age",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add the old column back
            migrationBuilder.AddColumn<int>(
                name: "age",
                table: "users",
                type: "integer",
                nullable: false);
            
            migrationBuilder.Sql(
                "UPDATE users SET age = DATE_PART('year', AGE(birth_date))");
        
            // Remove the new column
            migrationBuilder.DropColumn(
                name: "birth_date",
                table: "users");
        }
    }
}
