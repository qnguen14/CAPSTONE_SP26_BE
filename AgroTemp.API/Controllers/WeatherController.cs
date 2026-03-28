using AgroTemp.API.Constants;
using AgroTemp.Domain.DTO.Weather;
using AgroTemp.Domain.Metadata;
using AgroTemp.Service.Interfaces;
using AgroTemp.Service.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgroTemp.API.Controllers;

[ApiController]
public class WeatherController : Controller
{
    private readonly IWeatherService _weatherService;
    private readonly IUserService _userService;
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(
        IWeatherService weatherService,
        IUserService userService,
        ILogger<WeatherController> logger)
    {
        _weatherService = weatherService;
        _userService = userService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpointConstants.Weather.GetWeatherByCoordinatesEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<WeatherDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin weather by coordinates.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get weather by coordinates.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WeatherGetWeatherByCoordinates")]
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

    [AllowAnonymous]
    [HttpGet(ApiEndpointConstants.Weather.GetWeatherByCityEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<WeatherDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin weather by city.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get weather by city.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WeatherGetWeatherByCity")]
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

    [Authorize(Roles = "Farmer,Worker")]
    [HttpGet(ApiEndpointConstants.Weather.GetWeatherByCurrentUserAddressEndpoint)]
    [ProducesResponseType(typeof(ApiResponse<WeatherDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    [Microsoft.AspNetCore.Http.EndpointSummary("Lay thong tin weather by current user address.")]
    [Microsoft.AspNetCore.Http.EndpointDescription("Thuc hien chuc nang get weather by current user address.")]
    [Microsoft.AspNetCore.Routing.EndpointName("WeatherGetWeatherByCurrentUserAddress")]
    public async Task<ActionResult<WeatherDTO>> GetWeatherByCurrentUserAddress()
    {
        try
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "User not authenticated.",
                    Data = null
                });
            }

            var user = await _userService.GetUserById(userId);
            if (user == null || string.IsNullOrWhiteSpace(user.Address))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "User address is not set.",
                    Data = null
                });
            }

            var weather = await _weatherService.GetWeatherByAddressAsync(user.Address);

            return Ok(new ApiResponse<WeatherDTO>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Weather data retrieved successfully for current user address",
                Data = weather
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving weather for current user address");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = ex.Message,
                Data = null
            });
        }
    }
}
