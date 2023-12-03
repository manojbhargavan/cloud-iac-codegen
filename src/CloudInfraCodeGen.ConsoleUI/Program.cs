using CloudInfraCodeGen.Library.SKFacade;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using SKHelper.Lib;
using SKHelper.Lib.Models;
using System;

namespace CloudInfraCodeGen.ConsoleUI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var _loggerFactory = LoggerFactory.Create(builder =>
                { 
                    builder.AddConsole(); 
                    builder.SetMinimumLevel(LogLevel.Debug);
                });
                var kernal = new KernelHelper(_loggerFactory, kernalUsage: KernalUsage.ChatCompletion32k);
                string userInput = "What is the latest version of python?";

                var planner = new SequentialPlanner(kernal.Kernal);
                var plan = await planner.CreatePlanAsync(                    
                    $"Get top 5 web search results on the users intended topic: '{userInput}' and " +
                    $"save the search results to C:\\Users\\manoj\\source\\Pages as topic name.html. " +
                    $"Finally Upload all the html file to the storage account");
                plan.Steps.ToList().ForEach(s => Console.WriteLine(s.ToString()));
                var planResult = await kernal.Kernal.RunAsync(plan);
                Console.WriteLine(planResult);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
