using FluentAssertions;
using WeatherForecastApplication.Models;
using WeatherForecastApplication.Services.Validation;

namespace WeatherForecastApplication.Tests.Services;

public class WeatherForecastRequestValidatorTests
{
    private readonly WeatherForecastRequestValidator _validator;

    public WeatherForecastRequestValidatorTests()
    {
        _validator = new WeatherForecastRequestValidator();
    }

    [Fact]
    public async Task Validate_WithValidCoordinates_ShouldPass()
    {
        // Arrange
        var request = new WeatherForecastRequest
        {
            Latitude = 52.52,
            Longitude = 13.41
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(-90, 0)]
    [InlineData(90, 0)]
    [InlineData(0, -180)]
    [InlineData(0, 180)]
    [InlineData(45.5, 123.75)]
    public async Task Validate_WithBoundaryValidCoordinates_ShouldPass(double latitude, double longitude)
    {
        // Arrange
        var request = new WeatherForecastRequest
        {
            Latitude = latitude,
            Longitude = longitude
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(-100)]
    [InlineData(90.1)]
    [InlineData(100)]
    [InlineData(200)]
    public async Task Validate_WithInvalidLatitude_ShouldFail(double invalidLatitude)
    {
        // Arrange
        var request = new WeatherForecastRequest
        {
            Latitude = invalidLatitude,
            Longitude = 13.41
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Latitude");
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(-200)]
    [InlineData(180.1)]
    [InlineData(200)]
    [InlineData(360)]
    public async Task Validate_WithInvalidLongitude_ShouldFail(double invalidLongitude)
    {
        // Arrange
        var request = new WeatherForecastRequest
        {
            Latitude = 52.52,
            Longitude = invalidLongitude
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Longitude");
    }

    [Fact]
    public async Task Validate_WithBothInvalidCoordinates_ShouldFailWithMultipleErrors()
    {
        // Arrange
        var request = new WeatherForecastRequest
        {
            Latitude = 100,
            Longitude = 200
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.PropertyName == "Latitude");
        result.Errors.Should().Contain(e => e.PropertyName == "Longitude");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-45.5, -123.75)]
    [InlineData(35.6762, 139.6503)] // Tokyo
    [InlineData(40.7128, -74.0060)] // New York
    public async Task Validate_WithRealWorldCoordinates_ShouldPass(double latitude, double longitude)
    {
        // Arrange
        var request = new WeatherForecastRequest
        {
            Latitude = latitude,
            Longitude = longitude
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
