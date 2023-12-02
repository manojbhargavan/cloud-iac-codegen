using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Plugins;

namespace CloudInfraCodeGen.Library.SKFacade
{
    public class AzureOpenAIHelper
    {
        private readonly IKernel _kernel;

        public AzureOpenAIHelper(string azureEndpoint, string apiKey, string deploymentName)
        {
            var builder = new KernelBuilder();
            builder.WithAzureOpenAIChatCompletionService(deploymentName: deploymentName, endpoint: azureEndpoint, apiKey: apiKey);
            _kernel = builder.Build();
        }

        public async Task PrintRespose(string userInput)
        {
            var variables = new ContextVariables
            {
                ["input"] = "Yes",
                ["history"] = @"Bot: How can I help you?
                                    User: My team just hit a major milestone and I would like to send them a message to congratulate them.
                                    Bot: Would you like to send an email?",
                ["options"] = "SendEmail, ReadEmail, SendMeeting, RsvpToMeeting, SendChat"
            };

            var pluginDir = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Plugins");
            var plugin = _kernel.ImportSemanticFunctionsFromDirectory(pluginDir, "ChatPlugin");

            var result = await _kernel.RunAsync(variables, plugin["GetIntent"]);


            Console.WriteLine(result);
        }

    }
}
