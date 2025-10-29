using WeatherForcastApplication.Models;


namespace WeatherForcastApplication.Client
{
    public static class DataTranslators
    {
        public static WeatherForecastResponseDTO ToWeatherForecastResponse(this WeatherForecastProviderResponse response)
        {
            var weatherForecastResponse = new WeatherForecastResponseDTO
            {
                Latitude = response.latitude,
                Longitude = response.longitude,
                Elevation = response.elevation,
                GenerationTime = response.generationtime_ms,
                Timezone = response.timezone,
                TimezoneAbbreviation = response.timezone_abbreviation,
                UtcOffsetSeconds = response.utc_offset_seconds
            };
            AddNestedData(response, weatherForecastResponse);
            return weatherForecastResponse;
        }

        private static void AddNestedData(WeatherForecastProviderResponse providerResponse, WeatherForecastResponseDTO weatherForecastResponse)
        {
            AddDailyTimeSeries(providerResponse, weatherForecastResponse);
            AddHourlyTimeSeries(providerResponse, weatherForecastResponse);
        }

        private static void AddDailyTimeSeries(WeatherForecastProviderResponse providerResponse, WeatherForecastResponseDTO weatherForecastResponse)
        {
            var dailyTimeSeries = providerResponse.daily.time;
            if (dailyTimeSeries != null)
            {
                foreach (var propInfo in providerResponse.daily_units.GetType().GetProperties())
                {
                    if (!propInfo.Name.Equals("time"))
                    {
                        IEnumerable<object> values = (providerResponse.daily.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(propInfo.Name))?.GetValue(providerResponse.daily)) as IEnumerable<object>;
                        if (values != null)
                        {
                            int index = 0;
                            foreach (var value in values)
                            {
                                weatherForecastResponse.TimelyData.Add(new TimelyData()
                                {
                                    TimeSeriesType = TimeSeriesTypeEnum.Daily,
                                    Name = propInfo.Name,
                                    Value = value?.ToString(),
                                    Time = dailyTimeSeries[index]
                                });
                                index++;
                            }
                        }
                    }
                }
            }
        }
        
        private static void AddHourlyTimeSeries(WeatherForecastProviderResponse providerResponse, WeatherForecastResponseDTO weatherForecastResponse)
        {
            var horlyTimeSeries = providerResponse.hourly.time;

            if (horlyTimeSeries != null)
            {

                foreach (var propInfo in providerResponse.hourly_units.GetType().GetProperties())
                {
                    if (!propInfo.Name.Equals("time"))
                    {
                        IEnumerable<object> values = (providerResponse.hourly.GetType().GetProperties().FirstOrDefault(p => p.Name.Equals(propInfo.Name))?.GetValue(providerResponse.hourly)) as IEnumerable<object>;

                        if (values != null)
                        {
                            int index = 0;
                            foreach (var value in values)
                            {
                                weatherForecastResponse.TimelyData.Add(new TimelyData()
                                {
                                    TimeSeriesType = TimeSeriesTypeEnum.Hourly,
                                    Name = propInfo.Name,
                                    Value = value?.ToString(),
                                    Time = horlyTimeSeries[index]
                                });
                                index++;
                            }
                        }
                    }
                }
            }
        }
    }
}