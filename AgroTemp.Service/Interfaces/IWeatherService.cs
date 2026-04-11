using AgroTemp.Domain.DTO.Weather;

namespace AgroTemp.Service.Interfaces;

public interface IWeatherService
{
    Task<WeatherDTO> GetWeatherByCoordinatesAsync(double latitude, double longitude);
    Task<WeatherDTO> GetWeatherByCityAsync(string city);
    Task<WeatherDTO> GetWeatherByAddressAsync(string address);
}
