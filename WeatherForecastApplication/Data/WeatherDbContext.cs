using Microsoft.EntityFrameworkCore;
using WeatherForecastApplication.Data.Entities;

namespace WeatherForecastApplication.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
        {
        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<WeatherForecastData> WeatherForecasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Location configuration
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations");
                
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasPrecision(10, 7); // Support coordinates like -123.4567890

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasPrecision(10, 7);

                entity.Property(e => e.Elevation)
                    .HasPrecision(10, 2);

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => new { e.Latitude, e.Longitude })
                    .HasDatabaseName("IX_Location_Coordinates");

                // Relationship
                entity.HasMany(l => l.WeatherForecasts)
                    .WithOne(w => w.Location)
                    .HasForeignKey(w => w.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WeatherForecastData configuration
            modelBuilder.Entity<WeatherForecastData>(entity =>
            {
                entity.ToTable("WeatherForecasts");
                
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LocationId)
                    .IsRequired();

                entity.Property(e => e.ForecastDate)
                    .IsRequired();

                entity.Property(e => e.RetrievedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.TemperatureMax)
                    .HasPrecision(5, 2);

                entity.Property(e => e.TemperatureMin)
                    .HasPrecision(5, 2);

                entity.Property(e => e.PrecipitationSum)
                    .HasPrecision(6, 2);

                entity.HasIndex(e => new { e.LocationId, e.ForecastDate })
                    .HasDatabaseName("IX_WeatherForecast_Location_Date");

                entity.HasIndex(e => e.RetrievedAt)
                    .HasDatabaseName("IX_WeatherForecast_RetrievedAt");
            });
        }
    }
}
