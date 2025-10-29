using WeatherForcastApplication.Services.Validation;
using WeatherForcastApplication.Client;
using WeatherForcastApplication.Models;

namespace WeatherForcastApplication.Services
{
    public class WeatherService : Abstractions.IWeatherService
    {

        private readonly WeatherForecastRequestValidator _requestValidator;
        private readonly WeatherForecastProvider _weatherForecastProvider;

        public WeatherService(WeatherForecastProvider weatherForecastProvider, WeatherForecastRequestValidator requestValidator)
        {
            _requestValidator = requestValidator;
            _weatherForecastProvider = weatherForecastProvider;
        }
        public Task<WeatherForecastResponseDTO> GetWeatherForecastAsync(Models.WeatherForecastRequest weatherForecastRequest)
        {
            // TODO: Uncomment validation once exception handling is in place
            // var validationResult = _requestValidator.ValidateAsync(weatherForecastRequest);
            // if (validationResult.IsValid == false)
            // {
            //     var errorMessages = string.Join('&', validationResult.Errors.Select(e => e.ErrorMessage));
            //     throw new BaseApplicationException(HttpStatusCode.BadRequest, errorMessages);
            // }

            var weatherForecastResponse = _weatherForecastProvider.GetWeatherForecastAsync(weatherForecastRequest);
            // Best approach to map from DTO to DataModel

            // _weatherForecastDataStore.SaveUpdateWeatherForecastAsync(weatherForecastDataModel);
            return weatherForecastResponse;
        }
        
    }
}