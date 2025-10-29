namespace WeatherForecastApplication.Models
{
    public class WeatherForecastProviderResponse
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public double elevation { get; set; }
        public HourlyUnits hourly_units { get; set; }
        public Hourly hourly { get; set; }
        public DailyUnits daily_units { get; set; }
        public Daily daily { get; set; }
    }
}