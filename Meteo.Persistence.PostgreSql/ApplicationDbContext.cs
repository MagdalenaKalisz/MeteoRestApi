namespace Meteo.Persistence.PostgreSql
{
    using Meteo.Persistence.PostgreSql.Dao;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Database context for PostgreSQL persistence.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor with database options.
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {
        }

        /// <summary>
        /// Weather forecasts table.
        /// </summary>
        public DbSet<WeatherForecastDao> WeatherForecasts { get; set; }

        /// <summary>
        /// Weather forecast definitions table.
        /// </summary>
        public DbSet<WeatherForecastDefinitionDao> WeatherForecastDefinitions { get; set; }

        /// <summary>
        /// Daily weather forecasts table.
        /// </summary>
        public DbSet<WeatherForecastForDayDao> WeatherForecastForDays { get; set; }

        /// <summary>
        /// Outbox messages table.
        /// </summary>
        public DbSet<OutboxMessageDao> OutboxMessages { get; set; }

        /// <summary>
        /// Configures entity mappings.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherForecastDao>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();
                entity.Property(d => d.CreatedAt)
                      .HasConversion(
                        v => v.UtcDateTime, 
                        v => new DateTimeOffset(v, TimeSpan.Zero));

                entity.HasMany(e => e.Forecasts)
                      .WithOne(f => f.WeatherForecast)
                      .HasForeignKey(f => f.WeatherForecastId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WeatherForecastDefinitionDao>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();
                entity.Property(d => d.CreatedAt)
                      .HasConversion(
                        v => v.UtcDateTime, 
                        v => new DateTimeOffset(v, TimeSpan.Zero));

                entity.HasIndex(d => new { d.Latitude, d.Longitude })
                      .HasDatabaseName("IX_WeatherForecastDefinitions_LatLong");
            });

            modelBuilder.Entity<WeatherForecastForDayDao>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();
                entity.Property(d => d.ForecastDateTime)
                      .HasConversion(
                        v => v.UtcDateTime, 
                        v => new DateTimeOffset(v, TimeSpan.Zero));
            });

            modelBuilder.Entity<OutboxMessageDao>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.Property(d => d.OccurredAt)
                      .HasConversion(
                        v => v.UtcDateTime,
                        v => new DateTimeOffset(v, TimeSpan.Zero));

                // Adding index on OccurredAt for faster queries on event order
                entity.HasIndex(x => x.OccurredAt)
                      .HasDatabaseName("IX_OutboxMessages_OccurredAt");

                entity.Property(x => x.Type).HasMaxLength(255);
                entity.Property(x => x.Payload).IsRequired();
            });
        }
    }
}
