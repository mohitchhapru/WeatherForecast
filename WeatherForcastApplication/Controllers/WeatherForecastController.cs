using Microsoft.AspNetCore.Mvc;
using WeatherForcastApplication.Models;
using WeatherForcastApplication.Services;

namespace WeatherForcastApplication.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WeatherService _weatherService;
 

    public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [HttpPost("Get")]
    public async Task<IActionResult> Get([FromBody] WeatherForecastRequest weatherForecastRequest)
    {
        var weatherForecastData = await _weatherService.GetWeatherForecastAsync(weatherForecastRequest);
        return Ok(weatherForecastData);
    }
}
