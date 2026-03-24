using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Weather;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgroTemp.API.Controllers;

[ApiController]
[AllowAnonymous]
public class WeatherController : Controller
{
    private readonly IWeatherService _weatherService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _logger = logger;
    }

    [HttpGet(ApiEndpointConstants.Weather.GetWeatherByCoordinatesEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<WeatherDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WeatherDTO>> GetWeatherByCoordinates(
        [FromQuery] double lat,
        [FromQuery] double lon)
    {
        try
        {
            if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid coordinates. Latitude must be between -90 and 90, longitude between -180 and 180.",
                    Data = null
                });
            }

            var weather = await _weatherService.GetWeatherByCoordinatesAsync(lat, lon);

            return Ok(new ApiResponse<WeatherDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Weather data retrieved successfully",
                Data = weather
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weather for coordinates ({Lat}, {Lon})", lat, lon);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }

    [HttpGet(ApiEndpointConstants.Weather.GetWeatherByCityEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<WeatherDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WeatherDTO>> GetWeatherByCity([FromQuery] string city)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "City name is required.",
                    Data = null
                });
            }

            var weather = await _weatherService.GetWeatherByCityAsync(city);

            return Ok(new ApiResponse<WeatherDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Weather data retrieved successfully",
                Data = weather
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weather for city {City}", city);
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }
}
