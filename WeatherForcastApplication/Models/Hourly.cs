namespace WeatherForcastApplication.Models
{
    public class Hourly
    {
        public List<string> time { get; set; }
        public List<double> temperature_2m { get; set; }
        public List<int> relativehumidity_2m { get; set; }
        public List<double> dewpoint_2m { get; set; }
        public List<int> weathercode { get; set; }
        public List<double> pressure_msl { get; set; }
        public List<double> surface_pressure { get; set; }
    }
}