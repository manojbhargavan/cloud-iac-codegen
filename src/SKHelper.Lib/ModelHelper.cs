using Azure.AI.OpenAI;
using SKHelper.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SKHelper.Lib
{
    internal sealed class ModelHelper
    {
        /// <summary>
        /// Appropriate model for Completion/
        /// </summary>
        /// <param name="useCase"></param>
        /// <returns></returns>
        public string GetModel(KernalUsage usage)
        {
            var model = Environment.GetEnvironmentVariable("SK_AZURE_MODEL");
            if (string.IsNullOrEmpty(model))
            {
                // default to chat 35 turbo
                model = "gpt-35-turbo";
                switch (usage)
                {
                    case KernalUsage.ChatCompletionFast:
                        model = "gpt-35-turbo";
                        break;
                    case KernalUsage.ChatCompletion:
                        model = "gpt-4";
                        break;
                    case KernalUsage.TextCompletionFast:
                        model = "gpt-35-turbo-16k";
                        break;
                    case KernalUsage.TextCompletion:
                        model = "gpt-4-32k";
                        break;
                    case KernalUsage.TextEmbedding:
                        model = "text-embedding-ada-002";
                        break;
                }
            }

            return model;
        }
    }
}
