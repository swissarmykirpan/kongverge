using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedConfig : IKongPluginConfig
    {
        [JsonProperty("http_method")]
        public string HttpMethod { get; set; }

        [JsonProperty("remove")]
        public RequestTransformerAdvancedTransformBase Remove { get; set; } = new RequestTransformerAdvancedTransformBase();

        [JsonProperty("replace")]
        public RequestTransformerAdvancedTransformReplace Replace { get; set; } = new RequestTransformerAdvancedTransformReplace();

        [JsonProperty("rename")]
        public RequestTransformerAdvancedTransformBase Rename { get; set; } = new RequestTransformerAdvancedTransformBase();

        [JsonProperty("add")]
        public RequestTransformerAdvancedTransformBase Add { get; set; } = new RequestTransformerAdvancedTransformBase();

        [JsonProperty("append")]
        public RequestTransformerAdvancedTransformBase Append { get; set; } = new RequestTransformerAdvancedTransformBase();

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is RequestTransformerAdvancedConfig otherConfig)
            {
                return otherConfig.HttpMethod == HttpMethod
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
