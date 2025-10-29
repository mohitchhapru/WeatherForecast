using WeatherForecastApplication.Data.Entities;

namespace WeatherForecastApplication.Data.Repositories
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(int id);
        Task<Location?> GetByCoordinatesAsync(double latitude, double longitude);
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location> AddAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(double latitude, double longitude);
    }
}
