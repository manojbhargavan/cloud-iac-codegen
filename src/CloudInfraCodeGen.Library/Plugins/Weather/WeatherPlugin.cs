using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudInfraCodeGen.Library.Plugins.Weather
{
    public class WeatherPlugin
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public WeatherPlugin(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        [SKFunction, Description("get weather information based on the location")]        
        public string? GetWeatherAsync([Description("location or city name")]string cityName)
        {
            var apiUrl = $"https://api.weatherapi.com/v1/current.json?key={_apiKey}&q={cityName}&aqi=no";

            try
            {
                HttpResponseMessage response = _httpClient.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode();

                var responseBody = response.Content.ReadAsStringAsync().Result;
                return responseBody;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
