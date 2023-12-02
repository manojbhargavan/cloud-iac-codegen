using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace CloudInfraCodeGen.Library.SKFacade
{
    public class OpenAIHelper
    {
        private readonly IKernel _kernel;
        public OpenAIHelper(string model, string apiKey, string orgId)
        {
            var builder = new KernelBuilder();
            builder.WithOpenAIChatCompletionService(model, apiKey, orgId);
            _kernel = builder.Build();
        }
    }
}
