using Kongverge.Extension;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedConfig : IKongPluginConfig
    {
        [JsonProperty("consumer_id")]
        public string ConsumerId { get; set; }

        [JsonProperty("http_method")]
        public string HttpMethod { get; set; }

        [JsonProperty("remove")]
        public RequestTransformerAdvancedTransformRemove Remove { get; set; }

        [JsonProperty("replace")]
        public RequestTransformerAdvancedTransformReplace Replace { get; set; }

        [JsonProperty("rename")]
        public RequestTransformerAdvancedTransformKpBase Rename { get; set; }

        [JsonProperty("add")]
        public RequestTransformerAdvancedTransformKpBase Add { get; set; }

        [JsonProperty("append")]
        public RequestTransformerAdvancedTransformKpBase Append { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is RequestTransformerAdvancedConfig otherConfig)
            {
                return otherConfig.ConsumerId == ConsumerId
                       && otherConfig.HttpMethod == HttpMethod
                       && IsNestedConfigMatch(Remove, otherConfig.Remove)
                       && IsNestedConfigMatch(Replace, otherConfig.Replace)
                       && IsNestedConfigMatch(Rename, otherConfig.Rename)
                       && IsNestedConfigMatch(Add, otherConfig.Add)
                       && IsNestedConfigMatch(Append, otherConfig.Append);
            }

            return false;
        }

        private static bool IsNestedConfigMatch(
            IRequestTransformerAdvancedNestedConfig nestedConfig,
            IRequestTransformerAdvancedNestedConfig otherNestedConfig)
        {
            if (nestedConfig == null && otherNestedConfig == null)
            {
                return true;
            }

            if (nestedConfig != null)
            {
                return nestedConfig.IsExactMatch(otherNestedConfig);
            }

            return false;
        }
    }
}
