namespace WeatherForecastApplication.Models
{
    public class Daily
    {
        public List<string> time { get; set; }
        public List<int> weathercode { get; set; }
        public List<double> temperature_2m_max { get; set; }
        public List<double> temperature_2m_min { get; set; }
        public List<double> precipitation_sum { get; set; }
        public List<double> rain_sum { get; set; }
        public List<double> showers_sum { get; set; }
    }
}