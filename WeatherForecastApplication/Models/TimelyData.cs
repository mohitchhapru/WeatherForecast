namespace WeatherForecastApplication.Models
{
    public enum TimeSeriesTypeEnum
    {
        Daily,
        Hourly
    }
    public class TimelyData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string Value { get; set; }
        public TimeSeriesTypeEnum TimeSeriesType { get; set; }
    }
}