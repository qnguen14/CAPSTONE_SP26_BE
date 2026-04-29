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

    private const string WeatherBaseUrl         = "https://api.openweathermap.org/data/2.5/weather";
    private const string WeatherForecastBaseUrl = "https://api.openweathermap.org/data/2.5/forecast";
    private const string NominatimUrl           = "https://nominatim.openstreetmap.org/search";

    private const string NominatimAgent = "AgroTempApp/1.0 (contact@agrotemp.vn)";

    public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<WeatherDTO> GetWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        var query = $"lat={latitude}&lon={longitude}";
        return await FetchWeatherWithForecastAsync(query);
    }

    public async Task<WeatherDTO> GetWeatherByCityAsync(string city)
    {
        var query = $"q={Uri.EscapeDataString(city)}";
        return await FetchWeatherWithForecastAsync(query);
    }

    public async Task<WeatherDTO> GetWeatherByAddressAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required.", nameof(address));

        _logger.LogInformation("Geocoding address via Nominatim: {Address}", address);

        var (lat, lon) = await GeocodeWithNominatimAsync(address);
        return await GetWeatherByCoordinatesAsync(lat, lon);
    }

    // ── Nominatim geocoding ──────────────────────────────────────────────────
    private async Task<(double Lat, double Lon)> GeocodeWithNominatimAsync(string address)
    {
        var url = $"{NominatimUrl}?q={Uri.EscapeDataString(address)}&format=json&limit=1&countrycodes=vn";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", NominatimAgent);
        request.Headers.Add("Accept-Language", "vi,en");

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Network error calling Nominatim for address: {Address}", address);
            throw new Exception("Geocoding service is unavailable. Please try again later.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _logger.LogError("Nominatim returned {StatusCode}: {Body}", response.StatusCode, body);
            throw new HttpRequestException($"Geocoding API returned {(int)response.StatusCode}: {response.ReasonPhrase}");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
        {
            _logger.LogWarning("Nominatim returned no results for: {Address}", address);
            throw new Exception($"Could not find location for address: '{address}'. Please check the address and try again.");
        }

        var result = doc.RootElement[0];
        var lat    = double.Parse(result.GetProperty("lat").GetString()!, System.Globalization.CultureInfo.InvariantCulture);
        var lon    = double.Parse(result.GetProperty("lon").GetString()!, System.Globalization.CultureInfo.InvariantCulture);

        _logger.LogInformation("Nominatim resolved '{Address}' → ({Lat}, {Lon})", address, lat, lon);
        return (lat, lon);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private string GetApiKey()
    {
        var apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = _configuration["OpenWeather:SecretKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = Environment.GetEnvironmentVariable("OPEN_WEATHER_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = Environment.GetEnvironmentVariable("OPENWEATHER_SECRET_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenWeather API key is not configured.");
        return apiKey;
    }

    private async Task<WeatherDTO> FetchWeatherWithForecastAsync(string query)
    {
        try
        {
            var apiKey      = GetApiKey();
            var currentUrl  = $"{WeatherBaseUrl}?{query}&appid={apiKey}&units=metric&lang=vi";
            var forecastUrl = $"{WeatherForecastBaseUrl}?{query}&appid={apiKey}&units=metric&lang=vi";

            var currentResponse = await _httpClient.GetAsync(currentUrl);

            if (!currentResponse.IsSuccessStatusCode)
            {
                var errorBody = await currentResponse.Content.ReadAsStringAsync();
                _logger.LogError("OpenWeather current API error {StatusCode}: {Body}", currentResponse.StatusCode, errorBody);
                throw new HttpRequestException($"Weather API returned {(int)currentResponse.StatusCode}: {currentResponse.ReasonPhrase}");
            }

            var forecastResponse = await _httpClient.GetAsync(forecastUrl);

            if (!forecastResponse.IsSuccessStatusCode)
            {
                var errorBody = await forecastResponse.Content.ReadAsStringAsync();
                _logger.LogError("OpenWeather forecast API error {StatusCode}: {Body}", forecastResponse.StatusCode, errorBody);
                throw new HttpRequestException($"Forecast API returned {(int)forecastResponse.StatusCode}: {forecastResponse.ReasonPhrase}");
            }

            var currentJson  = await currentResponse.Content.ReadAsStringAsync();
            var forecastJson = await forecastResponse.Content.ReadAsStringAsync();

            using var currentDoc  = JsonDocument.Parse(currentJson);
            using var forecastDoc = JsonDocument.Parse(forecastJson);

            var weather = ParseCurrentWeather(currentDoc.RootElement);
            weather.HourlyForecast = ParseHourlyForecast(forecastDoc.RootElement);
            ApplyDailyRangeFromForecast(weather);

            return weather;
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not HttpRequestException)
        {
            _logger.LogError(ex, "Unexpected error fetching weather data");
            throw new Exception("Failed to retrieve weather data. Please try again later.", ex);
        }
    }

    private WeatherDTO ParseCurrentWeather(JsonElement root)
    {
        var main    = root.GetProperty("main");
        var weather = root.GetProperty("weather")[0];
        var wind    = root.GetProperty("wind");
        var sys     = root.GetProperty("sys");
        var coord   = root.GetProperty("coord");

        return new WeatherDTO
        {
            City        = root.GetProperty("name").GetString()   ?? string.Empty,
            Country     = sys.GetProperty("country").GetString() ?? string.Empty,
            Latitude    = coord.GetProperty("lat").GetDouble(),
            Longitude   = coord.GetProperty("lon").GetDouble(),
            Temperature = main.GetProperty("temp").GetDouble(),
            FeelsLike   = main.GetProperty("feels_like").GetDouble(),
            TempMin     = main.GetProperty("temp_min").GetDouble(),
            TempMax     = main.GetProperty("temp_max").GetDouble(),
            Humidity    = main.GetProperty("humidity").GetInt32(),
            WindSpeed   = wind.GetProperty("speed").GetDouble(),
            Description = weather.GetProperty("description").GetString() ?? string.Empty,
            Icon        = weather.GetProperty("icon").GetString()        ?? string.Empty,
            Sunrise     = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunrise").GetInt64()).UtcDateTime,
            Sunset      = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunset").GetInt64()).UtcDateTime,
            FetchedAt   = DateTime.UtcNow
        };
    }

    private static ICollection<WeatherForecastItemDTO> ParseHourlyForecast(JsonElement root)
    {
        var result = new List<WeatherForecastItemDTO>();

        if (!root.TryGetProperty("list", out var forecastList) || forecastList.ValueKind != JsonValueKind.Array)
        {
            return result;
        }

        var now = DateTime.UtcNow;
        var end = now.AddDays(5);

        foreach (var item in forecastList.EnumerateArray())
        {
            if (!item.TryGetProperty("dt", out var dtElement))
                continue;

            var forecastTime = DateTimeOffset.FromUnixTimeSeconds(dtElement.GetInt64()).UtcDateTime;
            if (forecastTime < now || forecastTime > end)
                continue;

            if (!item.TryGetProperty("main", out var main))
                continue;

            if (!item.TryGetProperty("weather", out var weatherList)
                || weatherList.ValueKind != JsonValueKind.Array
                || weatherList.GetArrayLength() == 0)
                continue;

            var weather = weatherList[0];

            var windSpeed = 0d;
            if (item.TryGetProperty("wind", out var wind)
                && wind.TryGetProperty("speed", out var speedElement)
                && speedElement.ValueKind == JsonValueKind.Number)
            {
                windSpeed = speedElement.GetDouble();
            }

            result.Add(new WeatherForecastItemDTO
            {
                ForecastTime = forecastTime,
                Temperature  = main.GetProperty("temp").GetDouble(),
                FeelsLike    = main.GetProperty("feels_like").GetDouble(),
                TempMin      = main.GetProperty("temp_min").GetDouble(),
                TempMax      = main.GetProperty("temp_max").GetDouble(),
                Humidity     = main.GetProperty("humidity").GetInt32(),
                WindSpeed    = windSpeed,
                Description  = weather.GetProperty("description").GetString() ?? string.Empty,
                Icon         = weather.GetProperty("icon").GetString()        ?? string.Empty
            });

            if (result.Count >= 40)
                break;
        }

        return result;
    }

    private static void ApplyDailyRangeFromForecast(WeatherDTO weather)
    {
        if (weather.HourlyForecast.Count == 0)
            return;

        var min = double.MaxValue;
        var max = double.MinValue;

        foreach (var hourly in weather.HourlyForecast)
        {
            if (hourly.TempMin < min) min = hourly.TempMin;
            if (hourly.TempMax > max) max = hourly.TempMax;
        }

        weather.TempMin = min;
        weather.TempMax = max;
    }
}
