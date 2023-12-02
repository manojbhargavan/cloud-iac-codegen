using Newtonsoft.Json;
using System.Text;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SKHelper.Lib.Plugins.WebSearchPlugin
{
    public class BingSearch
    {
        private readonly string _subscriptionKey;
        private readonly string _baseUri;
        private readonly string _chromeDriverPath;
        private readonly string _chromeBrowserPath;
        private const string QUERY_PARAMETER = "?q=";  // Required
        private const string MKT_PARAMETER = "&mkt=";  // Strongly suggested
        private const string RESPONSE_FILTER_PARAMETER = "&responseFilter=";
        private const string COUNT_PARAMETER = "&count=";
        private const string OFFSET_PARAMETER = "&offset=";
        private const string FRESHNESS_PARAMETER = "&freshness=";
        private const string SAFE_SEARCH_PARAMETER = "&safeSearch=";
        private const string TEXT_DECORATIONS_PARAMETER = "&textDecorations=";
        private const string TEXT_FORMAT_PARAMETER = "&textFormat=";
        private const string ANSWER_COUNT = "&answerCount=";
        private const string PROMOTE = "&promote=";
        private string _clientIdHeader;

        public BingSearch()
        {
            _subscriptionKey = Environment.GetEnvironmentVariable("BING_API_KEY") ?? "";
            _chromeDriverPath = Environment.GetEnvironmentVariable("CHROME_DRIVER_PATH") ?? "";
            _chromeBrowserPath = Environment.GetEnvironmentVariable("CHROME_BROWSER_PATH") ?? "";

            if (string.IsNullOrEmpty(_subscriptionKey))
            {
                throw new Exception("BING_API_KEY is not set for BingSearch. Set the same or disable the plugin in Functions.inf");
            }

            if (string.IsNullOrEmpty(_chromeDriverPath))
            {
                throw new Exception("CHROME_DRIVER_PATH is not set for BingSearch. Set the same or disable the plugin in Functions.inf");
            }

            if (string.IsNullOrEmpty(_chromeBrowserPath))
            {
                throw new Exception("CHROME_BROWSER_PATH is not set for BingSearch. Set the same or disable the plugin in Functions.inf");
            }

            _baseUri = "https://api.bing.microsoft.com/v7.0/search";
        }

        [SKFunction, Description("Search the web using Bing")]
        public async Task<string> Search([Description("Search string")]string searchString)
        {
            try
            {
                // Remember to encode query parameters like q, responseFilters, promote, etc.

                //var queryString = QUERY_PARAMETER + Uri.EscapeDataString(searchString) 
                //    + Uri.EscapeDataString("+site%3Aregistry.terraform.io/providers/hashicorp/azurerm/latest/docs");
                var queryString = QUERY_PARAMETER + Uri.EscapeDataString("azurerm+databricks+workspace+site:registry.terraform.io/providers/hashicorp/azurerm/latest/docs");
                queryString += MKT_PARAMETER + "en-us";
                queryString += RESPONSE_FILTER_PARAMETER + Uri.EscapeDataString("webpages");
                queryString += TEXT_DECORATIONS_PARAMETER + Boolean.FalseString;
                queryString += COUNT_PARAMETER + "10";

                HttpResponseMessage response = await MakeRequestAsync(queryString);
                var contentString = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<BinSearchResults>(contentString);

                if (response.IsSuccessStatusCode)
                {
                    StringBuilder resultPageContent = new("");
                    resultPageContent.Append($"Bing Search Results for: {searchString}\n\n");
                    var client = new HttpClient();
                    int resultCount = 1;
                    foreach (var result in searchResponse!.WebPages.Value)
                    {
                        var pageUri = result.Url;
                        var doc = GetPageContents(pageUri.ToString());

                        resultPageContent.Append($"Result {resultCount++}:\n");
                        resultPageContent.Append($"Title: {doc.ParsedText}\n");
                        resultCount++;
                    }
                    return resultPageContent.ToString();
                }
                else
                {
                    return "No response from Bing Search.";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<HttpResponseMessage> MakeRequestAsync(string queryString)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            return (await client.GetAsync(_baseUri + queryString));
        }

        HtmlDocument GetPageContents(string url)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.BinaryLocation = _chromeBrowserPath;

            // Create a new instance of the ChromeDriver
            using (var driver = new ChromeDriver(_chromeDriverPath, chromeOptions))
            {
                // Navigate to the URL
                driver.Navigate().GoToUrl(url);

                // Wait for the page to load (you might need to adjust the wait time)
                System.Threading.Thread.Sleep(5000);

                // Get the fully rendered HTML
                string html = driver.PageSource;

                // Now you can use HtmlAgilityPack to parse the HTML as before
                var document = new HtmlDocument();
                document.LoadHtml(html);

                // Perform scraping using HtmlAgilityPack or other methods

                // Close the browser window
                driver.Quit();
                return document;
            }
        }
    }
}
