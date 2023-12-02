using CsvHelper;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SKHelper.Lib.Models;
using SKHelper.Lib.Plugins.Weather;
using System.Globalization;

namespace SKHelper.Lib
{
    public sealed class KernelHelper
    {
        private readonly ILoggerFactory _logger;
        public IKernel Kernal { get; private set; }

        public string KernalModel { get; private set; }

        public KernelHelper(ILoggerFactory logger, bool isAzure = true, KernalUsage kernalUsage = KernalUsage.ChatCompletionFast)
        {
            KernelBuilder kernalBuilder = SetKernal(isAzure, kernalUsage);

            // Add Other Services
            _logger = logger;
            AddOtherServices(kernalBuilder);

            // Build Kernel
            Kernal = kernalBuilder.Build();

            // Register Functions
            RegisterFunctions();
        }

        private void RegisterFunctions()
        {
            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
            var pluginConfigFile = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "Config", "Functions.inf");
            using (var reader = new StreamReader(pluginConfigFile))
            using (var csvConfig = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var functionsConfig = csvConfig.GetRecords<FunctionsConfig>().ToList();
                foreach (var functionConfig in functionsConfig)
                {
                    if (functionConfig.Enabled)
                    {
                        if (functionConfig.Type.Equals("SemanticDir"))
                        {
                            Kernal.ImportSemanticFunctionsFromDirectory(pluginDir, functionConfig.Identifier);
                        }
                        else
                        {
                            // Create Instance of Type given Type Name
                            var type = Type.GetType(functionConfig.Identifier);
                            if (type == null)
                            {
                                throw new Exception($"Type {functionConfig.Identifier} not found");
                            }

                            var instance = Activator.CreateInstance(type);

                            if (instance == null)
                            {
                                throw new Exception($"Instance of type {functionConfig.Identifier} not created during Function Registration. Check Configurations");
                            }
                            Kernal.ImportFunctions(instance, type.Name);
                        }
                    }
                }
            }
        }

        private void AddOtherServices(KernelBuilder kernalBuilder)
        {
            kernalBuilder.WithLoggerFactory(_logger);
        }

        private KernelBuilder SetKernal(bool isAzure, KernalUsage kernalUsage)
        {
            var _modelHelper = new ModelHelper();
            KernalModel = _modelHelper.GetModel(kernalUsage);

            // Build Kernel
            var kernalBuilder = new KernelBuilder();

            if (isAzure)
            {
                SetAzureOpenAIKernal(kernalUsage, kernalBuilder);
            }
            else
            {
                SetOpenAIKernal(kernalUsage, kernalBuilder);
            }

            return kernalBuilder;
        }

        private void SetAzureOpenAIKernal(KernalUsage kernalUsage, KernelBuilder kernalBuilder)
        {
            var azureEndpoint = Environment.GetEnvironmentVariable("SK_AZURE_EP");
            var azureKey = Environment.GetEnvironmentVariable("SK_AZURE_KEY");

            if (string.IsNullOrEmpty(azureEndpoint) || string.IsNullOrEmpty(azureKey))
            {
                throw new Exception("Environment variables SK_AZURE_EP, SK_AZURE_KEY, SK_AZURE_MODEL must be set");
            }

            switch (kernalUsage)
            {
                case KernalUsage.ChatCompletionFast:
                case KernalUsage.ChatCompletion:
                default:
                    kernalBuilder.WithAzureOpenAIChatCompletionService(KernalModel, azureEndpoint, azureKey);
                    break;
                case KernalUsage.TextCompletionFast:
                case KernalUsage.TextCompletion:
                    kernalBuilder.WithAzureTextCompletionService(KernalModel, azureEndpoint, azureKey);
                    break;
                case KernalUsage.TextEmbedding:
                    kernalBuilder.WithAzureOpenAITextEmbeddingGenerationService(KernalModel, azureEndpoint, azureKey);
                    break;
            }
        }

        private void SetOpenAIKernal(KernalUsage kernalUsage, KernelBuilder kernalBuilder)
        {
            var openaiKey = Environment.GetEnvironmentVariable("SK_OPENAI_KEY");

            if (string.IsNullOrEmpty(openaiKey) || string.IsNullOrEmpty(KernalModel))
            {
                throw new Exception("Environment variables SK_OPENAI_KEY, SK_OPENAI_MODEL must be set");
            }

            switch (kernalUsage)
            {
                case KernalUsage.ChatCompletionFast:
                case KernalUsage.ChatCompletion:
                default:
                    kernalBuilder.WithOpenAIChatCompletionService(KernalModel, openaiKey);
                    break;
                case KernalUsage.TextCompletionFast:
                case KernalUsage.TextCompletion:
                    kernalBuilder.WithOpenAITextCompletionService(KernalModel, openaiKey);
                    break;
                case KernalUsage.TextEmbedding:
                    kernalBuilder.WithOpenAITextEmbeddingGenerationService(KernalModel, openaiKey);
                    break;
            }
        }
    }
}