using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeJustSayingDefaultsConfig : IKongPluginConfig
    {
        [JsonProperty("raisingcomponent")]
        public string RaisingComponent { get; set; }

        [JsonProperty("tenant")]
        public string Tenant { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is JeJustSayingDefaultsConfig otherConfig)
            {
                return
                    RaisingComponent == otherConfig.RaisingComponent &&
                    Tenant == otherConfig.Tenant;
            }
            return false;
        }

        public string id { get; set; }
    }
}
