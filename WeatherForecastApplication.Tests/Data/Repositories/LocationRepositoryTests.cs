using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecastApplication.Data;
using WeatherForecastApplication.Data.Entities;
using WeatherForecastApplication.Data.Repositories;

namespace WeatherForecastApplication.Tests.Data.Repositories;

public class LocationRepositoryTests : IDisposable
{
    private readonly WeatherDbContext _context;
    private readonly LocationRepository _repository;
    private readonly Mock<ILogger<LocationRepository>> _mockLogger;

    public LocationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WeatherDbContext(options);
        _mockLogger = new Mock<ILogger<LocationRepository>>();
        _repository = new LocationRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddLocationToDatabase()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Elevation = 38.0,
            Name = "Berlin",
            Description = "Capital of Germany"
        };

        // Act
        var result = await _repository.AddAsync(location);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Latitude.Should().Be(52.52);
        result.Longitude.Should().Be(13.41);
        result.Name.Should().Be("Berlin");

        var dbLocation = await _context.Locations.FindAsync(result.Id);
        dbLocation.Should().NotBeNull();
        dbLocation!.Latitude.Should().Be(52.52);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnLocation()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 40.7128,
            Longitude = -74.0060,
            Name = "New York"
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(location.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(location.Id);
        result.Name.Should().Be("New York");
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
    public async Task GetByCoordinatesAsync_WithExactMatch_ShouldReturnLocation()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = "Berlin"
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCoordinatesAsync(52.52, 13.41);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Berlin");
    }

    [Fact]
    public async Task GetByCoordinatesAsync_WithinTolerance_ShouldReturnLocation()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = "Berlin"
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();

        // Act - Search with coordinates within tolerance (0.0001 degrees)
        var result = await _repository.GetByCoordinatesAsync(52.520001, 13.410001);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Berlin");
    }

    [Fact]
    public async Task GetByCoordinatesAsync_OutsideTolerance_ShouldReturnNull()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = "Berlin"
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();

        // Act - Search with coordinates outside tolerance
        var result = await _repository.GetByCoordinatesAsync(52.53, 13.42);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllLocationsOrderedByLastAccessed()
    {
        // Arrange
        var location1 = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = "Berlin",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            LastAccessedAt = DateTime.UtcNow.AddDays(-2)
        };

        var location2 = new Location
        {
            Latitude = 40.7128,
            Longitude = -74.0060,
            Name = "New York",
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            LastAccessedAt = DateTime.UtcNow.AddDays(-1)
        };

        var location3 = new Location
        {
            Latitude = 35.6762,
            Longitude = 139.6503,
            Name = "Tokyo",
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            LastAccessedAt = null
        };

        await _context.Locations.AddRangeAsync(location1, location2, location3);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        var locationList = results.ToList();
        locationList.Should().HaveCount(3);
        locationList[0].Name.Should().Be("New York"); // Most recently accessed
        locationList[1].Name.Should().Be("Berlin");
        locationList[2].Name.Should().Be("Tokyo"); // Never accessed, ordered by CreatedAt
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateLocationProperties()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = "Berlin",
            Description = "Old description"
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();

        // Modify location
        location.Name = "Berlin Updated";
        location.Description = "New description";
        location.LastAccessedAt = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(location);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Berlin Updated");
        result.Description.Should().Be("New description");
        result.LastAccessedAt.Should().NotBeNull();

        // Verify in database
        var dbLocation = await _context.Locations.FindAsync(location.Id);
        dbLocation!.Name.Should().Be("Berlin Updated");
        dbLocation.Description.Should().Be("New description");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteLocationAndReturnTrue()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = "Berlin"
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();
        var locationId = location.Id;

        // Act
        var result = await _repository.DeleteAsync(locationId);

        // Assert
        result.Should().BeTrue();

        var deletedLocation = await _context.Locations.FindAsync(locationId);
        deletedLocation.Should().BeNull();
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
    public async Task ExistsAsync_WithExistingCoordinates_ShouldReturnTrue()
    {
        // Arrange
        var location = new Location
        {
            Latitude = 52.52,
            Longitude = 13.41,
            Name = "Berlin"
        };
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(52.52, 13.41);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingCoordinates_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(52.52, 13.41);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
