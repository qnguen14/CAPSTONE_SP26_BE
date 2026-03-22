using AgroTemp.Domain.DTO.Weather;
using AgroTemp.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AgroTemp.Service.Implements;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherService> _logger;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<WeatherDTO> GetWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        var apiKey = GetApiKey();
        var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=vi";
        return await FetchWeatherAsync(url);
    }

    public async Task<WeatherDTO> GetWeatherByCityAsync(string city)
    {
        var apiKey = GetApiKey();
        var url = $"{BaseUrl}?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric&lang=vi";
        return await FetchWeatherAsync(url);
    }

    private string GetApiKey()
    {
        var apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenWeather API key is not configured.");
        return apiKey;
    }

    private async Task<WeatherDTO> FetchWeatherAsync(string url)
    {
        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("OpenWeather API error {StatusCode}: {Body}", response.StatusCode, errorBody);
                throw new HttpRequestException($"Weather API returned {(int)response.StatusCode}: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var main = root.GetProperty("main");
            var weather = root.GetProperty("weather")[0];
            var wind = root.GetProperty("wind");
            var sys = root.GetProperty("sys");
            var coord = root.GetProperty("coord");

            return new WeatherDTO
            {
                City = root.GetProperty("name").GetString() ?? string.Empty,
                Country = sys.GetProperty("country").GetString() ?? string.Empty,
                Latitude = coord.GetProperty("lat").GetDouble(),
                Longitude = coord.GetProperty("lon").GetDouble(),
                Temperature = main.GetProperty("temp").GetDouble(),
                FeelsLike = main.GetProperty("feels_like").GetDouble(),
                TempMin = main.GetProperty("temp_min").GetDouble(),
                TempMax = main.GetProperty("temp_max").GetDouble(),
                Humidity = main.GetProperty("humidity").GetInt32(),
                WindSpeed = wind.GetProperty("speed").GetDouble(),
                Description = weather.GetProperty("description").GetString() ?? string.Empty,
                Icon = weather.GetProperty("icon").GetString() ?? string.Empty,
                Sunrise = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunrise").GetInt64()).UtcDateTime,
                Sunset = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunset").GetInt64()).UtcDateTime,
                FetchedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not HttpRequestException)
        {
            _logger.LogError(ex, "Unexpected error fetching weather data");
            throw new Exception("Failed to retrieve weather data. Please try again later.", ex);
        }
    }
}
