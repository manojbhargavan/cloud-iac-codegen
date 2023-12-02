using CloudInfraCodeGen.Library.Plugins.Weather;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudInfraCodeGen.Library.SKFacade
{
    public sealed class BasicFuntionPlugin
    {
        private readonly IKernel _kernel;
        private readonly IDictionary<string, ISKFunction> _weatherPlugin;

        public BasicFuntionPlugin(string deploymentName, string azureEndPoint, string apiKey, string weatherApiKey)
        {
            // Create and Build the Kernal
            _kernel = new KernelBuilder()
                .WithAzureOpenAIChatCompletionService(deploymentName, azureEndPoint, apiKey)
                .Build();

            // Import the Plugin
            _weatherPlugin = _kernel.ImportFunctions(new WeatherPlugin(weatherApiKey), nameof(WeatherPlugin));
        }

        public async Task GetUserIntent(string cityName)
        {
            // Create the 
            //var result = await _kernel.RunAsync("hyderabad", _weatherPlugin["GetWeatherAsync"]);
            //Console.Write(result.GetValue<string>());

            var planner = new SequentialPlanner(_kernel);
            var plan = await planner.CreatePlanAsync("i am planning to travel to chennai");
            var planResult = await _kernel.RunAsync(plan);
            Console.WriteLine(planResult);
        }
    }
}
