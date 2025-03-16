namespace Meteo.Persistence.PostgreSql
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.IO;

    /// <summary>
    /// Factory for creating ApplicationDbContext during design-time and runtime.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly IConfiguration? _configuration;

        /// <summary>
        /// Constructor for runtime dependency injection.
        /// </summary>
        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Parameterless constructor for EF Core migrations.
        /// </summary>
        public ApplicationDbContextFactory()
        {
            // Manually load configuration for design-time usage (EF Core Migrations)
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Ensure EF Core finds appsettings.json
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        /// Creates a new instance of ApplicationDbContext.
        /// </summary>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("Configuration is not available.");
            }

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing. Check appsettings.json or environment variables.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
