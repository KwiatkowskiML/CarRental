namespace CarRental.WebAPI.Configuration
{
    public class EnvironmentConfig
    {
        public DatabaseConfig Database { get; set; } = new();
    }

    public class DatabaseConfig
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        public string GetConnectionString()
        {
            return $"Host={Host};" +
                   $"Database={Name};" +
                   $"Username={Username};" +
                   $"Password={Password};" +
                   "SSL Mode=Require;Trust Server Certificate=true;";
        }
    }
}