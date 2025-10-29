namespace WeatherForcastApplication.Models
{
    public class WeatherForecastResponseDTO
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public double GenerationTime { get; set; }
        public int UtcOffsetSeconds { get; set; }
        public string? Timezone { get; set; }
        public string? TimezoneAbbreviation { get; set; }
        public List<TimelyData> TimelyData { get; set; } = new List<TimelyData>();
    }
}