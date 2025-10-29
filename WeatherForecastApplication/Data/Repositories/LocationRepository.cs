using Microsoft.EntityFrameworkCore;
using WeatherForecastApplication.Data.Entities;

namespace WeatherForecastApplication.Data.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly WeatherDbContext _context;
        private readonly ILogger<LocationRepository> _logger;

        public LocationRepository(WeatherDbContext context, ILogger<LocationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Locations
                .Include(l => l.WeatherForecasts)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Location?> GetByCoordinatesAsync(double latitude, double longitude)
        {
            // Check for exact match or very close match (within 0.0001 degrees ~11 meters)
            var tolerance = 0.0001;
            return await _context.Locations
                .FirstOrDefaultAsync(l => 
                    Math.Abs(l.Latitude - latitude) < tolerance && 
                    Math.Abs(l.Longitude - longitude) < tolerance);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                .OrderByDescending(l => l.LastAccessedAt ?? l.CreatedAt)
                .ToListAsync();
        }

        public async Task<Location> AddAsync(Location location)
        {
            location.CreatedAt = DateTime.UtcNow;
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added new location with Id: {LocationId}", location.Id);
            return location;
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated location with Id: {LocationId}", location.Id);
            return location;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return false;
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted location with Id: {LocationId}", id);
            return true;
        }

        public async Task<bool> ExistsAsync(double latitude, double longitude)
        {
            var tolerance = 0.0001;
            return await _context.Locations
                .AnyAsync(l => 
                    Math.Abs(l.Latitude - latitude) < tolerance && 
                    Math.Abs(l.Longitude - longitude) < tolerance);
        }
    }
}
