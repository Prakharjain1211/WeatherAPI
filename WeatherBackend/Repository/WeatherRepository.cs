using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class WeatherRepository : IWeatherRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WeatherRepository(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<WeatherDataDTO> GetCurrentWeatherAsync(string city)
    {
        var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_configuration["OpenWeather:ApiKey"]}&units=metric");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var weatherData = JsonConvert.DeserializeObject<dynamic>(content);

        return new WeatherDataDTO
        {
            City = city,
            Temperature = weatherData.main.temp,
            Description = weatherData.weather[0].description,
            Humidity = weatherData.main.humidity,
            WindSpeed = weatherData.wind.speed
        };
    }

    public async Task<WeatherDataDTO> GetCurrentWeatherByPincodeAsync(string pincode)
    {
        var response = await _httpClient.GetAsync(
            $"{_configuration["OpenWeather:BaseUrl"]}weather?zip={pincode},in&appid={_configuration["OpenWeather:ApiKey"]}&units=metric");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var weatherData = JsonConvert.DeserializeObject<dynamic>(content);

        return new WeatherDataDTO
        {
            City = weatherData.name,
            Temperature = weatherData.main.temp,
            Description = weatherData.weather[0].description,
            Humidity = weatherData.main.humidity,
            WindSpeed = weatherData.wind.speed
        };
    }

    public async Task<IEnumerable<WeatherForecastDTO>> GetWeatherForecastAsync(string city)
    {
        var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_configuration["OpenWeather:ApiKey"]}&units=metric");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var forecastData = JsonConvert.DeserializeObject<dynamic>(content);

        var forecasts = new List<WeatherForecastDTO>();

        int maxForecasts = 5;
        int count = 0;
        foreach (var item in forecastData.list)
        {
            if (count >= maxForecasts)
            {
                break;
            }

            forecasts.Add(new WeatherForecastDTO
            {
                Date = item.dt_txt,
                Temperature = item.main.temp,
                Description = item.weather[0].description
            });

            count++;
        }

        

        return forecasts;
    }

    public async Task<string> GetAirQualityAsync(string city)
    {
        var (lat, lon) = await GetCityCoordinatesAsync(city);
        var response = await _httpClient.GetAsync(
            $"{_configuration["OpenWeather:BaseUrl"]}air_pollution?lat={lat}&lon={lon}&appid={_configuration["OpenWeather:ApiKey"]}");
        response.EnsureSuccessStatusCode();
        var weather = await response.Content.ReadAsStringAsync();
        return weather;
    }

    private async Task<(double lat, double lon)> GetCityCoordinatesAsync(string city)
    {
        var response = await _httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_configuration["OpenWeather:ApiKey"]}");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync();
        var weatherData = JObject.Parse(data);

        double lat = (double)weatherData["coord"]["lat"];
        double lon = (double)weatherData["coord"]["lon"];

        return (lat, lon);
    }
}
