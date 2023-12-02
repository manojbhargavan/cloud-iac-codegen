using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Globalization;

namespace SKHelper.Lib.Plugins.WebSearchPlugin
{
    public partial class BinSearchResults
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("queryContext")]
        public QueryContext QueryContext { get; set; }

        [JsonProperty("webPages")]
        public WebPages WebPages { get; set; }

        [JsonProperty("rankingResponse")]
        public RankingResponse RankingResponse { get; set; }
    }

    public partial class QueryContext
    {
        [JsonProperty("originalQuery")]
        public string OriginalQuery { get; set; }
    }

    public partial class RankingResponse
    {
        [JsonProperty("mainline")]
        public Mainline Mainline { get; set; }
    }

    public partial class Mainline
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("answerType")]
        public AnswerType AnswerType { get; set; }

        [JsonProperty("resultIndex")]
        public long ResultIndex { get; set; }

        [JsonProperty("value")]
        public ItemValue Value { get; set; }
    }

    public partial class ItemValue
    {
        [JsonProperty("id")]
        public Uri Id { get; set; }
    }

    public partial class WebPages
    {
        [JsonProperty("webSearchUrl")]
        public Uri WebSearchUrl { get; set; }

        [JsonProperty("totalEstimatedMatches")]
        public long TotalEstimatedMatches { get; set; }

        [JsonProperty("value")]
        public List<ValueElement> Value { get; set; }
    }

    public partial class ValueElement
    {
        [JsonProperty("id")]
        public Uri Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("datePublished", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? DatePublished { get; set; }

        [JsonProperty("datePublishedDisplayText", NullValueHandling = NullValueHandling.Ignore)]
        public string DatePublishedDisplayText { get; set; }

        [JsonProperty("isFamilyFriendly")]
        public bool IsFamilyFriendly { get; set; }

        [JsonProperty("displayUrl")]
        public Uri DisplayUrl { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("dateLastCrawled")]
        public DateTimeOffset DateLastCrawled { get; set; }

        [JsonProperty("cachedPageUrl")]
        public Uri CachedPageUrl { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("isNavigational")]
        public bool IsNavigational { get; set; }

        [JsonProperty("contractualRules", NullValueHandling = NullValueHandling.Ignore)]
        public List<ContractualRule> ContractualRules { get; set; }
    }

    public partial class ContractualRule
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("targetPropertyName")]
        public string TargetPropertyName { get; set; }

        [JsonProperty("targetPropertyIndex")]
        public long TargetPropertyIndex { get; set; }

        [JsonProperty("mustBeCloseToContent")]
        public bool MustBeCloseToContent { get; set; }

        [JsonProperty("license")]
        public License License { get; set; }

        [JsonProperty("licenseNotice")]
        public string LicenseNotice { get; set; }
    }

    public partial class License
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public enum AnswerType { WebPages };

    public enum Language { En };

    public partial class BinSearchResults
    {
        public static BinSearchResults FromJson(string json) => JsonConvert.DeserializeObject<BinSearchResults>(json);
    }

    public static class Serialize
    {
        public static string ToJson(this BinSearchResults self) => JsonConvert.SerializeObject(self);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                AnswerTypeConverter.Singleton,
                LanguageConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class AnswerTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(AnswerType) || t == typeof(AnswerType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "WebPages")
            {
                return AnswerType.WebPages;
            }
            throw new Exception("Cannot unmarshal type AnswerType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (AnswerType)untypedValue;
            if (value == AnswerType.WebPages)
            {
                serializer.Serialize(writer, "WebPages");
                return;
            }
            throw new Exception("Cannot marshal type AnswerType");
        }

        public static readonly AnswerTypeConverter Singleton = new AnswerTypeConverter();
    }

    internal class LanguageConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Language) || t == typeof(Language?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "en")
            {
                return Language.En;
            }
            throw new Exception("Cannot unmarshal type Language");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Language)untypedValue;
            if (value == Language.En)
            {
                serializer.Serialize(writer, "en");
                return;
            }
            throw new Exception("Cannot marshal type Language");
        }

        public static readonly LanguageConverter Singleton = new LanguageConverter();
    }
}
