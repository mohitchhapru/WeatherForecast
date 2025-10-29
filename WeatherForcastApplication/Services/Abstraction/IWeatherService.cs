using WeatherForcastApplication.Models;

namespace WeatherForcastApplication.Services.Abstractions
{
    public interface IWeatherService
    {
        public Task<WeatherForecastResponseDTO> GetWeatherForecastAsync(WeatherForecastRequest weatherForecastRequest);
    }
}
