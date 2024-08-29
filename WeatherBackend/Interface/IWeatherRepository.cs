public interface IWeatherRepository
{
    Task<WeatherDataDTO> GetCurrentWeatherAsync(string city);
    Task<WeatherDataDTO> GetCurrentWeatherByPincodeAsync(string pincode);
    Task<IEnumerable<WeatherForecastDTO>> GetWeatherForecastAsync(string city);
    Task<string> GetAirQualityAsync(string city);
}
