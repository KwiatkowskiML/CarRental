using DotNetEnv;
using CarRental.WebAPI.Configuration;

namespace CarRental.WebAPI.Services
{
    public class EnvironmentService
    {
        private readonly EnvironmentConfig _config;
        private readonly ILogger<EnvironmentService> _logger;

        public EnvironmentService(ILogger<EnvironmentService> logger)
        {
            _logger = logger;
            
            try
            {
                Env.Load();
                _config = new EnvironmentConfig
                {
                    Database = new DatabaseConfig
                    {
                        Username = Env.GetString("DB_USERNAME"),
                        Password = Env.GetString("DB_PASSWORD"),
                        Host = Env.GetString("DB_HOST"),
                        Name = Env.GetString("DB_NAME")
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load environment variables");
                throw;
            }
        }

        public string GetConnectionString() => _config.Database.GetConnectionString();
    }
}
