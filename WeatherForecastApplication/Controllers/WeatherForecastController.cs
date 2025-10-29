using Microsoft.AspNetCore.Mvc;
using WeatherForecastApplication.Models;
using WeatherForecastApplication.Services.Abstractions;
using WeatherForecastApplication.Data.Repositories;

namespace WeatherForecastApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherService _weatherService;
    private readonly ILocationRepository _locationRepository;
 

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger, 
        IWeatherService weatherService,
        ILocationRepository locationRepository)
    {
        _logger = logger;
        _weatherService = weatherService;
        _locationRepository = locationRepository;
    }

    [HttpPost("Get")]
    public async Task<IActionResult> Get([FromBody] WeatherForecastRequest weatherForecastRequest)
    {
        var weatherForecastData = await _weatherService.GetWeatherForecastAsync(weatherForecastRequest);
        return Ok(weatherForecastData);
    }

    [HttpGet("Locations")]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await _locationRepository.GetAllAsync();
        return Ok(locations);
    }

    [HttpDelete("Locations/{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var deleted = await _locationRepository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Location with ID {id} not found." });
        }
        return Ok(new { message = $"Location with ID {id} deleted successfully." });
    }
    
}