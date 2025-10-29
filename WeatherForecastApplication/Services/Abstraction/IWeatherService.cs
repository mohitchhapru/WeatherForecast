using WeatherForecastApplication.Models;

namespace WeatherForecastApplication.Services.Abstractions
{
    public interface IWeatherService
    {
        public Task<WeatherForecastResponseDTO> GetWeatherForecastAsync(WeatherForecastRequest weatherForecastRequest);
    }
}
