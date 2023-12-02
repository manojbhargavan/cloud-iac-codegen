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
                var kernal = new KernelHelper(_loggerFactory, kernalUsage: KernalUsage.ChatCompletion);

                var planner = new SequentialPlanner(kernal.Kernal);
                var plan = await planner.CreatePlanAsync("How to create a Synpase workspace using terraform");
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
