using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherRepository _weatherRepository;

    public WeatherController(IWeatherRepository weatherRepository)
    {
        _weatherRepository = weatherRepository;
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentWeather([FromQuery] string city)
    {
        if (string.IsNullOrEmpty(city))
        {
            return BadRequest("City is required.");
        }

        try
        {
            var weather = await _weatherRepository.GetCurrentWeatherAsync(city);
            if (weather == null)
            {
                return NotFound($"No weather data found for city: {city}");
            }
            
            return Ok(weather);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, "Weather service is unavailable. Please try again later.");
        }
        catch (Exception ex)
        {
            // Log exception details (if logging is set up)
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("currentByPincode")]
    public async Task<IActionResult> GetCurrentWeatherByPincode([FromQuery] string pincode)
    {
        if (string.IsNullOrEmpty(pincode))
        {
            return BadRequest("Pincode is required.");
        }

        try
        {
            var weather = await _weatherRepository.GetCurrentWeatherByPincodeAsync(pincode);
            if (weather == null)
            {
                return NotFound($"No weather data found for pincode: {pincode}");
            }
            return Ok(weather);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, "Weather service is unavailable. Please try again later.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("forecast")]
    public async Task<IActionResult> GetWeatherForecast([FromQuery] string city)
    {
        if (string.IsNullOrEmpty(city))
        {
            return BadRequest("City is required.");
        }

        try
        {
            var forecast = await _weatherRepository.GetWeatherForecastAsync(city);
            if (forecast == null)
            {
                return NotFound($"No forecast data found for city: {city}");
            }
            return Ok(forecast);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, "Weather service is unavailable. Please try again later.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("airquality")]
    public async Task<IActionResult> GetAirQuality([FromQuery] string city)
    {
        if (string.IsNullOrEmpty(city))
        {
            return BadRequest("City is required.");
        }

        try
        {
            var airQuality = await _weatherRepository.GetAirQualityAsync(city);
            if (airQuality == null)
            {
                return NotFound($"No air quality data found for city: {city}");
            }
            return Ok(airQuality);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, "Weather service is unavailable. Please try again later.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
