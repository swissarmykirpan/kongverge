using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public enum RateLimitingIdentifier
    {
        // ReSharper disable InconsistentNaming
        // This is the default, so it comes first
        consumer,
        // ReSharper disable once InconsistentNaming
        ip,
        credential
        // ReSharper restore InconsistentNaming
    }

    public class RateLimitingConfig : IKongPluginConfig
    {
        [JsonProperty("limit")]
        public int[] Limit { get; set; } = new int[0];

        [JsonProperty("window_size")]
        public int[] WindowSize { get; set; } = new int[0];

        [JsonProperty("identifier")]
        public RateLimitingIdentifier Identifier { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is RateLimitingConfig otherConfig)
            {
                return otherConfig.Limit == Limit &&
                       otherConfig.WindowSize == WindowSize &&
                       otherConfig.Identifier == Identifier;
            }

            return false;
        }
    }
}
