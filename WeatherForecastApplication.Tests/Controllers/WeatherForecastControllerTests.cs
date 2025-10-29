using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherForecastApplication.Controllers;
using WeatherForecastApplication.Data.Entities;
using WeatherForecastApplication.Data.Repositories;
using WeatherForecastApplication.Models;
using WeatherForecastApplication.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace WeatherForecastApplication.Tests.Controllers;

public class WeatherForecastControllerTests
{
    private readonly Mock<ILogger<WeatherForecastController>> _mockLogger;
    private readonly Mock<IWeatherService> _mockWeatherService;
    private readonly Mock<ILocationRepository> _mockLocationRepository;
    private readonly WeatherForecastController _controller;

    public WeatherForecastControllerTests()
    {
        _mockLogger = new Mock<ILogger<WeatherForecastController>>();
        _mockWeatherService = new Mock<IWeatherService>();
        _mockLocationRepository = new Mock<ILocationRepository>();

        // Create controller with services interface instead of concrete implementation
        _controller = new WeatherForecastController(
            _mockLogger.Object,
            _mockWeatherService.Object,
            _mockLocationRepository.Object
        );
    }

    [Fact]
    public async Task Get_WithValidRequest_ShouldReturnOkWithWeatherData()
    {
        // Arrange
        var request = new WeatherForecastRequest
        {
            Latitude = 52.52,
            Longitude = 13.41
        };

        var expectedResponse = new WeatherForecastResponseDTO
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Elevation = 38.0,
            Timezone = "Europe/Berlin",
            TimelyData = new List<TimelyData>
            {
                new TimelyData { TimeSeriesType = "Daily", Name = "temperature_2m_max", Value = "22.5", Time = "2025-10-29" }
            }
        };

        _mockWeatherService.Setup(s => s.GetWeatherForecastAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Get(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var responseData = okResult.Value.Should().BeOfType<WeatherForecastResponseDTO>().Subject;
        responseData.Latitude.Should().Be(52.52);
        responseData.Longitude.Should().Be(13.41);
        responseData.TimelyData.Should().HaveCount(1);

        _mockWeatherService.Verify(s => s.GetWeatherForecastAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetLocations_ShouldReturnOkWithAllLocations()
    {
        // Arrange
        var locations = new List<Location>
        {
            new Location
            {
                Id = 1,
                Latitude = 52.52,
                Longitude = 13.41,
                Name = "Berlin",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Location
            {
                Id = 2,
                Latitude = 40.7128,
                Longitude = -74.0060,
                Name = "New York",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        _mockLocationRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(locations);

        // Act
        var result = await _controller.GetLocations();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var locationList = okResult.Value.Should().BeAssignableTo<IEnumerable<Location>>().Subject.ToList();
        locationList.Should().HaveCount(2);
        locationList[0].Name.Should().Be("Berlin");
        locationList[1].Name.Should().Be("New York");

        _mockLocationRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLocations_WithNoLocations_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _mockLocationRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Location>());

        // Act
        var result = await _controller.GetLocations();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var locationList = okResult.Value.Should().BeAssignableTo<IEnumerable<Location>>().Subject;
        locationList.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteLocation_WithExistingId_ShouldReturnOkWithSuccessMessage()
    {
        // Arrange
        var locationId = 1;
        _mockLocationRepository.Setup(r => r.DeleteAsync(locationId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteLocation(locationId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().NotBeNull();
        var messageProperty = okResult.Value!.GetType().GetProperty("message");
        messageProperty.Should().NotBeNull();
        var message = messageProperty!.GetValue(okResult.Value)?.ToString();
        message.Should().Contain("deleted successfully");
        message.Should().Contain(locationId.ToString());

        _mockLocationRepository.Verify(r => r.DeleteAsync(locationId), Times.Once);
    }

    [Fact]
    public async Task DeleteLocation_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var locationId = 999;
        _mockLocationRepository.Setup(r => r.DeleteAsync(locationId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteLocation(locationId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().NotBeNull();
        var messageProperty = notFoundResult.Value!.GetType().GetProperty("message");
        messageProperty.Should().NotBeNull();
        var message = messageProperty!.GetValue(notFoundResult.Value)?.ToString();
        message.Should().Contain("not found");
        message.Should().Contain(locationId.ToString());

        _mockLocationRepository.Verify(r => r.DeleteAsync(locationId), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public async Task DeleteLocation_WithVariousIds_ShouldCallRepositoryWithCorrectId(int locationId)
    {
        // Arrange
        _mockLocationRepository.Setup(r => r.DeleteAsync(locationId))
            .ReturnsAsync(true);

        // Act
        await _controller.DeleteLocation(locationId);

        // Assert
        _mockLocationRepository.Verify(r => r.DeleteAsync(locationId), Times.Once);
    }
}
