namespace WeatherForcastApplication.Models
{
    public class DailyUnits
    {
        public string time { get; set; }
        public string weathercode { get; set; }
        public string temperature_2m_max { get; set; }
        public string temperature_2m_min { get; set; }
        public string precipitation_sum { get; set; }
        public string rain_sum { get; set; }
        public string showers_sum { get; set; }
    }
}