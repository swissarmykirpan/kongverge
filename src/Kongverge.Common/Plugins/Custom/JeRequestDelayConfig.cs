using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeRequestDelayConfig : IKongPluginConfig
    {
        [JsonProperty("delay")]
        public long Delay { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is JeRequestDelayConfig otherConfig)
            {
                return
                    Delay == otherConfig.Delay;
            }
            return false;
        }

        public string id { get; set; }
    }
}
