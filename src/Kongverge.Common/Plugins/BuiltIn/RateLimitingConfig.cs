using Kongverge.Extension;
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

        public bool IsExactmatch(IKongPluginConfig other)
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
