using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeRequestDelayConfig : IKongPluginConfig
    {
        [JsonProperty("delay")]
        public int DelayMillis { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is JeRequestDelayConfig otherConfig)
            {
                return
                    DelayMillis == otherConfig.DelayMillis;
            }
            return false;
        }

        public string id { get; set; }
    }
}