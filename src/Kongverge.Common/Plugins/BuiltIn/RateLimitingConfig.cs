using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RateLimitingConfig : IKongPluginConfig
    {
        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("window_size")]
        public int WindowSize { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is RateLimitingConfig otherConfig)
            {
                return otherConfig.Limit == Limit
                       && otherConfig.WindowSize == WindowSize
                       && otherConfig.Identifier == Identifier;
            }

            return false;
        }
    }
}
