namespace AgroTemp.Domain.DTO.Weather;

public class WeatherForecastItemDTO
{
    public DateTime ForecastTime { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string IconUrl => $"https://openweathermap.org/img/wn/{Icon}@2x.png";
}

public class WeatherDTO
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string IconUrl => $"https://openweathermap.org/img/wn/{Icon}@2x.png";
    public DateTime Sunrise { get; set; }
    public DateTime Sunset { get; set; }
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
    public ICollection<WeatherForecastItemDTO> HourlyForecast { get; set; } = new List<WeatherForecastItemDTO>();
}
