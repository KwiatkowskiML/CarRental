using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CarRental.WebAPI.Services.Options;
using WebAPI.Auth;
using WebAPI.Data.Context;
using WebAPI.Data.Repositories.Interfaces;
using WebAPI.PriceCalculators;
using WebAPI.Services;
using WebAPI.Services.Interfaces;
using WebAPI.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add configuration source
builder.Configuration.AddEnvironmentVariables();

// Database configuration with null checks
var instanceConnectionName = builder.Configuration["INSTANCE_CONNECTION_NAME"] 
    ?? throw new InvalidOperationException("INSTANCE_CONNECTION_NAME is not configured");
var dbName = builder.Configuration["DB_NAME"] 
    ?? throw new InvalidOperationException("DB_NAME is not configured");
var dbUser = builder.Configuration["DB_USER"] 
    ?? throw new InvalidOperationException("DB_USER is not configured");
var dbPass = builder.Configuration["DB_PASS"] 
    ?? throw new InvalidOperationException("DB_PASS is not configured");

var connectionString = 
    $"Host={instanceConnectionName};" +
    $"Database={dbName};" +
    $"Username={dbUser};" +
    $"Password={dbPass};" +
    "SSL Mode=Disable";

builder.Services.AddDbContext<CarRentalContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Configuration with null checks
var jwtSecret = builder.Configuration["JWT_SECRET"] 
    ?? throw new InvalidOperationException("JWT_SECRET is not configured");
var jwtIssuer = builder.Configuration["JWT_ISSUER"] 
    ?? throw new InvalidOperationException("JWT_ISSUER is not configured");
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] 
    ?? throw new InvalidOperationException("JWT_AUDIENCE is not configured");

builder.Services.Configure<JwtOptions>(options => {
    options.Secret = jwtSecret;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
    options.ExpiryMinutes = int.Parse(builder.Configuration["JWT_EXPIRY_MINUTES"] ?? "60");
});

// Google Auth Configuration with null checks
builder.Services.Configure<GoogleAuthOptions>(options => {
    options.ClientId = builder.Configuration["GOOGLE_CLIENT_ID"] 
        ?? throw new InvalidOperationException("GOOGLE_CLIENT_ID is not configured");
    options.ClientSecret = builder.Configuration["GOOGLE_CLIENT_SECRET"] 
        ?? throw new InvalidOperationException("GOOGLE_CLIENT_SECRET is not configured");
});

// Email Configuration with null checks
builder.Services.Configure<EmailOptions>(options => {
    options.ApiKey = builder.Configuration["EMAIL_API_KEY"] 
        ?? throw new InvalidOperationException("EMAIL_API_KEY is not configured");
    options.FromEmail = builder.Configuration["EMAIL_FROM_EMAIL"] 
        ?? throw new InvalidOperationException("EMAIL_FROM_EMAIL is not configured");
    options.FromName = builder.Configuration["EMAIL_FROM_NAME"] 
        ?? throw new InvalidOperationException("EMAIL_FROM_NAME is not configured");
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Price Calculator
builder.Services.AddScoped<IPriceCalculator, PriceCalculator>();

// Register Repositories and Services
builder.Services.AddScoped<GoogleAuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRentalConfirmationService, RentalConfirmationService>();
builder.Services.AddHostedService<OfferCleanupService>();

// Register HttpClient for SendGrid
builder.Services.AddHttpClient();

// Register providers
builder.Services.AddScoped<IExternalCarProvider, SuperRentalCarProvider>();
builder.Services.AddScoped<CarProviderAggregator>();

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