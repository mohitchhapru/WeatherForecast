using WeatherForecastApplication.Models;


namespace WeatherForecastApplication.Client
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
            if (providerResponse?.daily?.time == null || !providerResponse.daily.time.Any())
            {
                return;
            }

            var dailyTimeSeries = providerResponse.daily.time;
            var daily = providerResponse.daily;

            // Temperature Max
            if (daily.temperature_2m_max != null)
            {
                for (int i = 0; i < Math.Min(dailyTimeSeries.Count, daily.temperature_2m_max.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Daily",
                        Name = "temperature_2m_max",
                        Value = daily.temperature_2m_max[i].ToString(),
                        Time = dailyTimeSeries[i]
                    });
                }
            }

            // Temperature Min
            if (daily.temperature_2m_min != null)
            {
                for (int i = 0; i < Math.Min(dailyTimeSeries.Count, daily.temperature_2m_min.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Daily",
                        Name = "temperature_2m_min",
                        Value = daily.temperature_2m_min[i].ToString(),
                        Time = dailyTimeSeries[i]
                    });
                }
            }

            // Weather Code
            if (daily.weathercode != null)
            {
                for (int i = 0; i < Math.Min(dailyTimeSeries.Count, daily.weathercode.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Daily",
                        Name = "weathercode",
                        Value = daily.weathercode[i].ToString(),
                        Time = dailyTimeSeries[i]
                    });
                }
            }

            // Precipitation Sum
            if (daily.precipitation_sum != null)
            {
                for (int i = 0; i < Math.Min(dailyTimeSeries.Count, daily.precipitation_sum.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Daily",
                        Name = "precipitation_sum",
                        Value = daily.precipitation_sum[i].ToString(),
                        Time = dailyTimeSeries[i]
                    });
                }
            }

            // Rain Sum
            if (daily.rain_sum != null)
            {
                for (int i = 0; i < Math.Min(dailyTimeSeries.Count, daily.rain_sum.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Daily",
                        Name = "rain_sum",
                        Value = daily.rain_sum[i].ToString(),
                        Time = dailyTimeSeries[i]
                    });
                }
            }

            // Showers Sum
            if (daily.showers_sum != null)
            {
                for (int i = 0; i < Math.Min(dailyTimeSeries.Count, daily.showers_sum.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Daily",
                        Name = "showers_sum",
                        Value = daily.showers_sum[i].ToString(),
                        Time = dailyTimeSeries[i]
                    });
                }
            }
        }
        
        private static void AddHourlyTimeSeries(WeatherForecastProviderResponse providerResponse, WeatherForecastResponseDTO weatherForecastResponse)
        {
            if (providerResponse?.hourly?.time == null || !providerResponse.hourly.time.Any())
            {
                return;
            }

            var hourlyTimeSeries = providerResponse.hourly.time;
            var hourly = providerResponse.hourly;

            // Temperature 2m
            if (hourly.temperature_2m != null)
            {
                for (int i = 0; i < Math.Min(hourlyTimeSeries.Count, hourly.temperature_2m.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Hourly",
                        Name = "temperature_2m",
                        Value = hourly.temperature_2m[i].ToString(),
                        Time = hourlyTimeSeries[i]
                    });
                }
            }

            // Relative Humidity 2m
            if (hourly.relativehumidity_2m != null)
            {
                for (int i = 0; i < Math.Min(hourlyTimeSeries.Count, hourly.relativehumidity_2m.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Hourly",
                        Name = "relativehumidity_2m",
                        Value = hourly.relativehumidity_2m[i].ToString(),
                        Time = hourlyTimeSeries[i]
                    });
                }
            }

            // Dewpoint 2m
            if (hourly.dewpoint_2m != null)
            {
                for (int i = 0; i < Math.Min(hourlyTimeSeries.Count, hourly.dewpoint_2m.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Hourly",
                        Name = "dewpoint_2m",
                        Value = hourly.dewpoint_2m[i].ToString(),
                        Time = hourlyTimeSeries[i]
                    });
                }
            }

            // Weather Code
            if (hourly.weathercode != null)
            {
                for (int i = 0; i < Math.Min(hourlyTimeSeries.Count, hourly.weathercode.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Hourly",
                        Name = "weathercode",
                        Value = hourly.weathercode[i].ToString(),
                        Time = hourlyTimeSeries[i]
                    });
                }
            }

            // Pressure MSL
            if (hourly.pressure_msl != null)
            {
                for (int i = 0; i < Math.Min(hourlyTimeSeries.Count, hourly.pressure_msl.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Hourly",
                        Name = "pressure_msl",
                        Value = hourly.pressure_msl[i].ToString(),
                        Time = hourlyTimeSeries[i]
                    });
                }
            }

            // Surface Pressure
            if (hourly.surface_pressure != null)
            {
                for (int i = 0; i < Math.Min(hourlyTimeSeries.Count, hourly.surface_pressure.Count); i++)
                {
                    weatherForecastResponse.TimelyData.Add(new TimelyData
                    {
                        TimeSeriesType = "Hourly",
                        Name = "surface_pressure",
                        Value = hourly.surface_pressure[i].ToString(),
                        Time = hourlyTimeSeries[i]
                    });
                }
            }
        }
    }
}