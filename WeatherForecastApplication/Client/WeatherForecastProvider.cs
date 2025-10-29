using WeatherForecastApplication.Models;

namespace WeatherForecastApplication.Client
{
    public class WeatherForecastProvider
    {
        private readonly OpenMeteoClient _openMeteoClient;
        public WeatherForecastProvider(OpenMeteoClient openMeteoClient)
        {
            _openMeteoClient = openMeteoClient;
        }
        public async Task<WeatherForecastResponseDTO> GetWeatherForecastAsync(WeatherForecastRequest weatherForecastRequest)
        {
            var response = await _openMeteoClient.GetForecastAsync(weatherForecastRequest);
            return response.ToWeatherForecastResponse();
        }
    }
}