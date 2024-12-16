using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CarRental.WebAPI.Data.Context;
using CarRental.WebAPI.Auth;
using CarRental.WebAPI.Data.Models;
using CarRental.WebAPI.Services;
using CarRental.WebAPI.Services.Interfaces;
using CarRental.WebAPI.Services.Options;
using WebAPI.Data.Mappers;
using WebAPI.Data.Repositories;
using WebAPI.Data.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext
builder.Services.AddDbContext<CarRentalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Authentication
builder.Services.Configure<GoogleAuthOptions>(
    builder.Configuration.GetSection("GoogleAuth"));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

// Configure Email
builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection("Email"));

// Register Repositories and Services
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRentalConfirmationService, RentalConfirmationService>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register HttpClient for SendGrid
builder.Services.AddHttpClient();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? 
                    throw new InvalidOperationException("Jwt:Secret is not configured")))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add CORS middleware
app.UseCors();

// Add authentication middleware before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();