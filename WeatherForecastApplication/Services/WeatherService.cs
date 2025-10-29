using WeatherForecastApplication.Services.Validation;
using WeatherForecastApplication.Client;
using WeatherForecastApplication.Models;
using WeatherForecastApplication.Common;
using WeatherForecastApplication.Data.Repositories;
using WeatherForecastApplication.Data.Entities;
using Newtonsoft.Json;
using System.Net;

namespace WeatherForecastApplication.Services
{
    public class WeatherService : Abstractions.IWeatherService
    {

        private readonly WeatherForecastRequestValidator _requestValidator;
        private readonly WeatherForecastProvider _weatherForecastProvider;
        private readonly ILocationRepository _locationRepository;
        private readonly IWeatherForecastRepository _weatherForecastRepository;

        public WeatherService(
            WeatherForecastProvider weatherForecastProvider, 
            WeatherForecastRequestValidator requestValidator,
            ILocationRepository locationRepository,
            IWeatherForecastRepository weatherForecastRepository)
        {
            _requestValidator = requestValidator;
            _weatherForecastProvider = weatherForecastProvider;
            _locationRepository = locationRepository;
            _weatherForecastRepository = weatherForecastRepository;
        }
        public async Task<WeatherForecastResponseDTO> GetWeatherForecastAsync(Models.WeatherForecastRequest weatherForecastRequest)
        {
            // Validate request
            var validationResult = await _requestValidator.ValidateAsync(weatherForecastRequest);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errorMessages = string.Join('&', validationResult.Errors.Select(e => e.ErrorMessage));
                throw new AppApplicationException(HttpStatusCode.BadRequest, errorMessages);
            }

            // Fetch weather forecast from provider
            var weatherForecastResponse = await _weatherForecastProvider.GetWeatherForecastAsync(weatherForecastRequest);

            // Get or create location
            var location = await _locationRepository.GetByCoordinatesAsync(
                weatherForecastRequest.Latitude, 
                weatherForecastRequest.Longitude);

            if (location == null)
            {
                location = new Location
                {
                    Latitude = weatherForecastRequest.Latitude,
                    Longitude = weatherForecastRequest.Longitude,
                    Elevation = weatherForecastResponse.Elevation,
                    CreatedAt = DateTime.UtcNow,
                    LastAccessedAt = DateTime.UtcNow
                };
                location = await _locationRepository.AddAsync(location);
            }
            else
            {
                // Update last accessed time
                location.LastAccessedAt = DateTime.UtcNow;
                await _locationRepository.UpdateAsync(location);
            }

            // Prepare weather forecast data for storage
            var weatherForecastData = new WeatherForecastData
            {
                LocationId = location.Id,
                ForecastDate = DateTime.UtcNow.Date,
                RetrievedAt = DateTime.UtcNow,
                Timezone = weatherForecastResponse.Timezone,
                TimezoneAbbreviation = weatherForecastResponse.TimezoneAbbreviation,
                HourlyDataJson = JsonConvert.SerializeObject(weatherForecastResponse.TimelyData
                    .Where(td => td.TimeSeriesType == "Hourly")
                    .ToList()),
                DailyDataJson = JsonConvert.SerializeObject(weatherForecastResponse.TimelyData
                    .Where(td => td.TimeSeriesType == "Daily")
                    .ToList())
            };

            // Extract summary data for quick access
            var tempMaxData = weatherForecastResponse.TimelyData
                .FirstOrDefault(td => td.TimeSeriesType == "Daily" && td.Name == "temperature_2m_max");
            var tempMinData = weatherForecastResponse.TimelyData
                .FirstOrDefault(td => td.TimeSeriesType == "Daily" && td.Name == "temperature_2m_min");
            var precipData = weatherForecastResponse.TimelyData
                .FirstOrDefault(td => td.TimeSeriesType == "Daily" && td.Name == "precipitation_sum");
            var weatherCodeData = weatherForecastResponse.TimelyData
                .FirstOrDefault(td => td.TimeSeriesType == "Daily" && td.Name == "weathercode");

            if (tempMaxData != null && double.TryParse(tempMaxData.Value, out var tempMax))
                weatherForecastData.TemperatureMax = tempMax;

            if (tempMinData != null && double.TryParse(tempMinData.Value, out var tempMin))
                weatherForecastData.TemperatureMin = tempMin;

            if (precipData != null && double.TryParse(precipData.Value, out var precip))
                weatherForecastData.PrecipitationSum = precip;

            if (weatherCodeData != null)
                weatherForecastData.WeatherCode = weatherCodeData.Value;

            // Save to database
            await _weatherForecastRepository.AddAsync(weatherForecastData);

            return weatherForecastResponse;
        }
        
    }
}