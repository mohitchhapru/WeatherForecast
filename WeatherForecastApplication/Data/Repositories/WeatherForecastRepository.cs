using Microsoft.EntityFrameworkCore;
using WeatherForecastApplication.Data.Entities;

namespace WeatherForecastApplication.Data.Repositories
{
    public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly WeatherDbContext _context;
        private readonly ILogger<WeatherForecastRepository> _logger;

        public WeatherForecastRepository(WeatherDbContext context, ILogger<WeatherForecastRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WeatherForecastData?> GetByIdAsync(int id)
        {
            return await _context.WeatherForecasts
                .Include(w => w.Location)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WeatherForecastData?> GetLatestByLocationIdAsync(int locationId)
        {
            return await _context.WeatherForecasts
                .Where(w => w.LocationId == locationId)
                .OrderByDescending(w => w.RetrievedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WeatherForecastData>> GetByLocationIdAsync(int locationId)
        {
            return await _context.WeatherForecasts
                .Where(w => w.LocationId == locationId)
                .OrderByDescending(w => w.RetrievedAt)
                .ToListAsync();
        }

        public async Task<WeatherForecastData> AddAsync(WeatherForecastData weatherForecast)
        {
            weatherForecast.RetrievedAt = DateTime.UtcNow;
            _context.WeatherForecasts.Add(weatherForecast);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added weather forecast with Id: {ForecastId} for Location: {LocationId}", 
                weatherForecast.Id, weatherForecast.LocationId);
            return weatherForecast;
        }

        public async Task<WeatherForecastData> UpdateAsync(WeatherForecastData weatherForecast)
        {
            _context.WeatherForecasts.Update(weatherForecast);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated weather forecast with Id: {ForecastId}", weatherForecast.Id);
            return weatherForecast;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var forecast = await _context.WeatherForecasts.FindAsync(id);
            if (forecast == null)
            {
                return false;
            }

            _context.WeatherForecasts.Remove(forecast);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted weather forecast with Id: {ForecastId}", id);
            return true;
        }

        public async Task<bool> DeleteByLocationIdAsync(int locationId)
        {
            var forecasts = await _context.WeatherForecasts
                .Where(w => w.LocationId == locationId)
                .ToListAsync();

            if (!forecasts.Any())
            {
                return false;
            }

            _context.WeatherForecasts.RemoveRange(forecasts);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted {Count} weather forecasts for Location: {LocationId}", 
                forecasts.Count, locationId);
            return true;
        }
    }
}
