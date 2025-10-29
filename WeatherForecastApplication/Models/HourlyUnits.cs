namespace WeatherForecastApplication.Models
{
    public class HourlyUnits
    {
        public string time { get; set; }
        public string temperature_2m { get; set; }
        public string relativehumidity_2m { get; set; }
        public string dewpoint_2m { get; set; }
        public string weathercode { get; set; }
        public string pressure_msl { get; set; }
        public string surface_pressure { get; set; }
    }
}