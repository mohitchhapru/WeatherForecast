using FluentValidation;
using WeatherForecastApplication.Models;

namespace WeatherForecastApplication.Services.Validation
{
    public class WeatherForecastRequestValidator : AbstractValidator<WeatherForecastRequest>
    {
        public WeatherForecastRequestValidator()
        {
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("StartDate must be less than or equal to EndDate.");

            RuleFor(x => x.ForecastDays)
                .GreaterThan(0)
                .When(x => x.ForecastDays.HasValue)
                .WithMessage("ForecastDays must be greater than 0 if specified.");
        }
    }
}