using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SKHelper.Lib.Plugins.Weather
{
    public class WeatherPlugin
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public WeatherPlugin()
        {
            _apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY") ?? "";
            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new Exception("WEATHER_API_KEY is not set for WeatherPlugin. Set the same or disable the plugin in Functions.inf");
            }
            _httpClient = new HttpClient();
        }

        [SKFunction, Description("get weather information based on the location")]
        public async Task<string> GetWeather([Description("location or city name")] string location)
        {
            var url = $"https://api.weatherapi.com/v1/current.json?key={_apiKey}&q={location}&aqi=no";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                //var weather = JsonSerializer.Deserialize<WeatherResponse>(content);
                //return $"The weather in {location} is {weather.TempC} degrees celcius";
                return content;
            }
            catch (Exception ex)
            {
                return $"Error getting weather for {location}: {ex.Message}";
            }
        }
    }
}
