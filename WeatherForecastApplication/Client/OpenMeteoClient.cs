using WeatherForecastApplication.Common;
using WeatherForecastApplication.Models;
using System.Text;

namespace WeatherForecastApplication.Client
{
    public class OpenMeteoClient
    {
        private readonly Webcaller _webcaller;
        public OpenMeteoClient(Webcaller webcaller)
        {
            _webcaller = webcaller;
        }

        public async Task<WeatherForecastProviderResponse> GetForecastAsync(WeatherForecastRequest weatherForecastRequest)
        {
            var url = GetUrl(weatherForecastRequest);

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };

            return await _webcaller.SendAsync<WeatherForecastProviderResponse>(httpRequestMessage);
        }

        public string GetUrl(WeatherForecastRequest weatherForecastRequest)
        {
            var queryParams = new List<string>
            {
                $"latitude={weatherForecastRequest.Latitude}",
                $"longitude={weatherForecastRequest.Longitude}"
            };

            if (!string.IsNullOrWhiteSpace(weatherForecastRequest.Timezone))
                queryParams.Add($"timezone={weatherForecastRequest.Timezone}");

            if (!string.IsNullOrWhiteSpace(weatherForecastRequest.TemperatureUnit))
                queryParams.Add($"temperature_unit={weatherForecastRequest.TemperatureUnit}");

            if (!string.IsNullOrWhiteSpace(weatherForecastRequest.PrecipitationUnit))
                queryParams.Add($"precipitation_unit={weatherForecastRequest.PrecipitationUnit}");

            var hourlyVariable = weatherForecastRequest.HourlyVariables != null && weatherForecastRequest.HourlyVariables.Any()
                ? string.Join(',', weatherForecastRequest.HourlyVariables)
                : string.Empty;
            if (!string.IsNullOrEmpty(hourlyVariable))
                queryParams.Add($"hourly={hourlyVariable}");

            var dailyVariables = weatherForecastRequest.DailyVariables != null && weatherForecastRequest.DailyVariables.Any()
                ? string.Join(',', weatherForecastRequest.DailyVariables)
                : string.Empty;
            if (!string.IsNullOrEmpty(dailyVariables))
                queryParams.Add($"daily={dailyVariables}");

            if (weatherForecastRequest.ForecastDays > 0)
            {
                queryParams.Add($"forecast_days={weatherForecastRequest.ForecastDays}");
            }
            else
            {
                if (weatherForecastRequest.StartDate.HasValue)
                    queryParams.Add($"start_date={weatherForecastRequest.StartDate.Value:yyyy-MM-dd}");
                if (weatherForecastRequest.EndDate.HasValue)
                    queryParams.Add($"end_date={weatherForecastRequest.EndDate.Value:yyyy-MM-dd}");
            }

            var url = $"https://api.open-meteo.com/v1/forecast?{string.Join("&", queryParams)}";
            return url;
        }

    }
}