using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherForecastApplication.Data.Entities
{
    public class WeatherForecastData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public DateTime ForecastDate { get; set; }

        [Required]
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? Timezone { get; set; }

        [MaxLength(20)]
        public string? TimezoneAbbreviation { get; set; }

        // Store serialized JSON data for flexibility
        [Column(TypeName = "TEXT")]
        public string? HourlyDataJson { get; set; }

        [Column(TypeName = "TEXT")]
        public string? DailyDataJson { get; set; }

        [Column(TypeName = "TEXT")]
        public string? CurrentDataJson { get; set; }

        // Commonly accessed daily summary fields (denormalized for quick access)
        public double? TemperatureMax { get; set; }

        public double? TemperatureMin { get; set; }

        public double? PrecipitationSum { get; set; }

        [MaxLength(10)]
        public string? WeatherCode { get; set; }

        // Foreign key
        [ForeignKey(nameof(LocationId))]
        public Location Location { get; set; } = null!;
    }
}
