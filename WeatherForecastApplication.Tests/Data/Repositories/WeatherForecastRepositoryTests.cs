using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecastApplication.Data;
using WeatherForecastApplication.Data.Entities;
using WeatherForecastApplication.Data.Repositories;

namespace WeatherForecastApplication.Tests.Data.Repositories;

public class WeatherForecastRepositoryTests : IDisposable
{
    private readonly WeatherDbContext _context;
    private readonly WeatherForecastRepository _repository;
    private readonly Mock<ILogger<WeatherForecastRepository>> _mockLogger;

    public WeatherForecastRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WeatherDbContext(options);
        _mockLogger = new Mock<ILogger<WeatherForecastRepository>>();
        _repository = new WeatherForecastRepository(_context, _mockLogger.Object);
    }

    private async Task<Location> CreateTestLocation(string name = "Test Location")
    {
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = name
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();
        return location;
    }

    [Fact]
    public async Task AddAsync_ShouldAddWeatherForecastToDatabase()
    {
        // Arrange
        var location = await CreateTestLocation();

        var weatherForecast = new WeatherForecastData
        {
            LocationId = location.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow,
            Timezone = "Europe/Berlin",
            TimezoneAbbreviation = "CET",
            HourlyDataJson = "[{\"temp\":20}]",
            DailyDataJson = "[{\"temp_max\":25}]",
            TemperatureMax = 25.5,
            TemperatureMin = 12.3,
            PrecipitationSum = 5.2,
            WeatherCode = "61"
        };

        // Act
        var result = await _repository.AddAsync(weatherForecast);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.LocationId.Should().Be(location.Id);
        result.Timezone.Should().Be("Europe/Berlin");
        result.TemperatureMax.Should().Be(25.5);

        var dbForecast = await _context.WeatherForecasts.FindAsync(result.Id);
        dbForecast.Should().NotBeNull();
        dbForecast!.TemperatureMax.Should().Be(25.5);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnWeatherForecast()
    {
        // Arrange
        var location = await CreateTestLocation();
        var weatherForecast = new WeatherForecastData
        {
            LocationId = location.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow,
            Timezone = "Europe/Berlin"
        };
        await _context.WeatherForecasts.AddAsync(weatherForecast);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(weatherForecast.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(weatherForecast.Id);
        result.Timezone.Should().Be("Europe/Berlin");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetLatestByLocationIdAsync_ShouldReturnMostRecentForecast()
    {
        // Arrange
        var location = await CreateTestLocation();

        var oldForecast = new WeatherForecastData
        {
            LocationId = location.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow.AddDays(-2),
            Timezone = "Europe/Berlin",
            TemperatureMax = 20.0
        };

        var latestForecast = new WeatherForecastData
        {
            LocationId = location.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow,
            Timezone = "Europe/Berlin",
            TemperatureMax = 25.0
        };

        await _context.WeatherForecasts.AddRangeAsync(oldForecast, latestForecast);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLatestByLocationIdAsync(location.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TemperatureMax.Should().Be(25.0);
        result.RetrievedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetLatestByLocationIdAsync_WithNoForecasts_ShouldReturnNull()
    {
        // Arrange
        var location = await CreateTestLocation();

        // Act
        var result = await _repository.GetLatestByLocationIdAsync(location.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByLocationIdAsync_ShouldReturnAllForecastsForLocation()
    {
        // Arrange
        var location1 = await CreateTestLocation("Location 1");
        var location2 = await CreateTestLocation("Location 2");

        var forecast1 = new WeatherForecastData
        {
            LocationId = location1.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow.AddDays(-2),
            TemperatureMax = 20.0
        };

        var forecast2 = new WeatherForecastData
        {
            LocationId = location1.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow.AddDays(-1),
            TemperatureMax = 22.0
        };

        var forecast3 = new WeatherForecastData
        {
            LocationId = location2.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow,
            TemperatureMax = 25.0
        };

        await _context.WeatherForecasts.AddRangeAsync(forecast1, forecast2, forecast3);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetByLocationIdAsync(location1.Id);

        // Assert
        var forecastList = results.ToList();
        forecastList.Should().HaveCount(2);
        forecastList.Should().AllSatisfy(f => f.LocationId.Should().Be(location1.Id));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateWeatherForecastProperties()
    {
        // Arrange
        var location = await CreateTestLocation();
        var weatherForecast = new WeatherForecastData
        {
            LocationId = location.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow,
            TemperatureMax = 20.0,
            TemperatureMin = 10.0
        };
        await _context.WeatherForecasts.AddAsync(weatherForecast);
        await _context.SaveChangesAsync();

        // Modify forecast
        weatherForecast.TemperatureMax = 25.0;
        weatherForecast.TemperatureMin = 15.0;
        weatherForecast.WeatherCode = "80";

        // Act
        var result = await _repository.UpdateAsync(weatherForecast);

        // Assert
        result.Should().NotBeNull();
        result.TemperatureMax.Should().Be(25.0);
        result.TemperatureMin.Should().Be(15.0);
        result.WeatherCode.Should().Be("80");

        // Verify in database
        var dbForecast = await _context.WeatherForecasts.FindAsync(weatherForecast.Id);
        dbForecast!.TemperatureMax.Should().Be(25.0);
        dbForecast.WeatherCode.Should().Be("80");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteForecastAndReturnTrue()
    {
        // Arrange
        var location = await CreateTestLocation();
        var weatherForecast = new WeatherForecastData
        {
            LocationId = location.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow
        };
        await _context.WeatherForecasts.AddAsync(weatherForecast);
        await _context.SaveChangesAsync();
        var forecastId = weatherForecast.Id;

        // Act
        var result = await _repository.DeleteAsync(forecastId);

        // Assert
        result.Should().BeTrue();

        var deletedForecast = await _context.WeatherForecasts.FindAsync(forecastId);
        deletedForecast.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteByLocationIdAsync_ShouldDeleteAllForecastsForLocation()
    {
        // Arrange
        var location1 = await CreateTestLocation("Location 1");
        var location2 = await CreateTestLocation("Location 2");

        var forecast1 = new WeatherForecastData
        {
            LocationId = location1.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow.AddDays(-2)
        };

        var forecast2 = new WeatherForecastData
        {
            LocationId = location1.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow.AddDays(-1)
        };

        var forecast3 = new WeatherForecastData
        {
            LocationId = location2.Id,
            ForecastDate = DateTime.UtcNow.Date,
            RetrievedAt = DateTime.UtcNow
        };

        await _context.WeatherForecasts.AddRangeAsync(forecast1, forecast2, forecast3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteByLocationIdAsync(location1.Id);

        // Assert
        result.Should().BeTrue();

        var remainingForecasts = await _context.WeatherForecasts.ToListAsync();
        remainingForecasts.Should().HaveCount(1);
        remainingForecasts[0].LocationId.Should().Be(location2.Id);
    }

    [Fact]
    public async Task DeleteByLocationIdAsync_WithNoForecasts_ShouldReturnFalse()
    {
        // Arrange
        var location = await CreateTestLocation();

        // Act
        var result = await _repository.DeleteByLocationIdAsync(location.Id);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
