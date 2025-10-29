using System.ComponentModel.DataAnnotations;

namespace WeatherForecastApplication.Data.Entities
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        public double? Elevation { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastAccessedAt { get; set; }

        // Navigation property
        public ICollection<WeatherForecastData> WeatherForecasts { get; set; } = new List<WeatherForecastData>();
    }
}
