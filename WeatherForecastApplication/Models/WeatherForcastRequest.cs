namespace WeatherForecastApplication.Models
{
    public class WeatherForecastRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<string>? HourlyVariables { get; set; } = new List<string>();
        public List<string>? DailyVariables { get; set; } = new List<string>();
        public string? TemperatureUnit { get; set; }
        public string? PrecipitationUnit { get; set; }
        public string? Timezone { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ForecastDays { get; set; }
    }
}