using WeatherForecastApplication.Data.Entities;

namespace WeatherForecastApplication.Data.Repositories
{
    public interface IWeatherForecastRepository
    {
        Task<WeatherForecastData?> GetByIdAsync(int id);
        Task<WeatherForecastData?> GetLatestByLocationIdAsync(int locationId);
        Task<IEnumerable<WeatherForecastData>> GetByLocationIdAsync(int locationId);
        Task<WeatherForecastData> AddAsync(WeatherForecastData weatherForecast);
        Task<WeatherForecastData> UpdateAsync(WeatherForecastData weatherForecast);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByLocationIdAsync(int locationId);
    }
}
