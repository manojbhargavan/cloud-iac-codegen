using Microsoft.Extensions.Logging;
using SKHelper.Lib;
using SKHelper.Lib.Models;

namespace SKHelper.Lib.Tests
{
    public class AzureChatCompletionKernelHelperTests
    {
        private readonly ILoggerFactory _loggerFactory;

        public AzureChatCompletionKernelHelperTests()
        {
            _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        }

        // Test that the constructor throws an exception if the environment variables are not set
        [Fact]
        public void Constructor_ThrowsException_IfEnvironmentVariablesNotSet_Azure()
        {
            // Arrange                        
            Environment.SetEnvironmentVariable("SK_AZURE_EP", "");
            Environment.SetEnvironmentVariable("SK_AZURE_KEY", "");
            Environment.SetEnvironmentVariable("SK_AZURE_MODEL", "");

            // Act
            var exception = Record.Exception(() => new KernelHelper(_loggerFactory));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
            Assert.Equal("Environment variables SK_AZURE_EP, SK_AZURE_KEY, SK_AZURE_MODEL must be set", exception.Message);
        }

        [Fact]
        public void Constructor_ThrowsException_IfEnvironmentVariablesNotSet_OpenAI()
        {
            // Arrange                        
            Environment.SetEnvironmentVariable("SK_OPENAI_KEY", "");
            Environment.SetEnvironmentVariable("SK_OPENAI_MODEL", "");

            // Act
            var exception = Record.Exception(() => new KernelHelper(_loggerFactory, false));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
            Assert.Equal("Environment variables SK_OPENAI_KEY, SK_OPENAI_MODEL must be set", exception.Message);
        }

        [Fact]
        public void Constructor_DoesNotThrowException_IfEnvironmentVariablesAreSet_Azure()
        {
            // Arrange                        
            Environment.SetEnvironmentVariable("SK_AZURE_EP", "https://endpoint");
            Environment.SetEnvironmentVariable("SK_AZURE_KEY", "key");
            Environment.SetEnvironmentVariable("SK_AZURE_MODEL", "model");

            // Act
            var exception = Record.Exception(() => new KernelHelper(_loggerFactory));

            // Assert
            Assert.Null(exception);
        }

        // Test that constructor does not throw an exception if the environment variables are set
        [Fact]
        public void Constructor_DoesNotThrowException_IfEnvironmentVariablesAreSet_AzureChatCompletionFast()
        {
            // Arrange                        
            
            // Act
            var exception = Record.Exception(() => new KernelHelper(_loggerFactory, kernalUsage: KernalUsage.ChatCompletionFast));

            // Assert
            Assert.Null(exception);
        }


        [Fact]
        public void Constructor_DoesNotThrowException_IfEnvironmentVariablesAreSet_OpenAI()
        {
            // Arrange                        
            Environment.SetEnvironmentVariable("SK_OPENAI_KEY", "key");
            Environment.SetEnvironmentVariable("SK_OPENAI_MODEL", "model");

            // Act
            var exception = Record.Exception(() => new KernelHelper(_loggerFactory, false));

            // Assert
            Assert.Null(exception);
        }

        // Check that Passing in a KernalUsage of TextEmbedding sets the correct model
        [Fact]
        public void Constructor_ReturnsCorrectModel_IfKernalUsageIsChatCompletionFast_Azure()
        {
            // Arrange                        
            Environment.SetEnvironmentVariable("SK_AZURE_EP", "https://endpoint");
            Environment.SetEnvironmentVariable("SK_AZURE_KEY", "key");

            // Act
            var helper = new KernelHelper(_loggerFactory, kernalUsage: KernalUsage.TextEmbedding);

            // Assert
            Assert.Equal("text-embedding-ada-002", helper.KernalModel);
        }

        // Check that Passing in a KernalUsage of ChatCompletionFast sets the correct model
        [Fact]
        public void Constructor_ReturnsCorrectModel_IfKernalUsageIsChatCompletionFast_OpenAI()
        {
            // Arrange                        
            Environment.SetEnvironmentVariable("SK_OPENAI_KEY", "key");

            // Act
            var helper = new KernelHelper(_loggerFactory, false, KernalUsage.ChatCompletionFast);

            // Assert
            Assert.Equal("gpt-35-turbo", helper.KernalModel);
        }

        // Check that Passing in a KernalUsage of ChatCompletion sets the correct model
        [Fact]
        public void Constructor_ReturnsCorrectModel_IfKernalUsageIsChatCompletion_Azure()
        {
            // Arrange                        
            Environment.SetEnvironmentVariable("SK_AZURE_EP", "https://endpoint");
            Environment.SetEnvironmentVariable("SK_AZURE_KEY", "key");

            // Act
            var helper = new KernelHelper(_loggerFactory, kernalUsage: KernalUsage.ChatCompletion);

            // Assert
            Assert.Equal("gpt-4", helper.KernalModel);
        }

    }
}