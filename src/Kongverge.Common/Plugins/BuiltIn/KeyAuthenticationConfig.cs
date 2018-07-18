using System.Collections.Generic;
using Kongverge.Extension;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class KeyAuthenticationConfig : IKongPluginConfig
    {
        [JsonProperty("key_names")]
        public HashSet<string> KeyNames { get; set; } = new HashSet<string> { "apikey" };

        [JsonProperty("key_in_body")]
        public bool KeyInBody { get; set; }

        [JsonProperty("hide_credentials")]
        public bool HideCredentials { get; set; }

        [JsonProperty("anonymous")]
        public string Anonymous { get; set; } = string.Empty;

        [JsonProperty("run_on_preflight")]
        public bool RunOnPreflight { get; set; } = true;

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is KeyAuthenticationConfig otherConfig)
            {
                return otherConfig.KeyInBody == KeyInBody
                    && otherConfig.HideCredentials == HideCredentials
                    && otherConfig.RunOnPreflight == RunOnPreflight
                    && otherConfig.Anonymous.Equals(Anonymous)
                    && otherConfig.KeyNames.Count == KeyNames.Count
                    && KeyNamesMatch(otherConfig);
            }

            return false;
        }

        private bool KeyNamesMatch(KeyAuthenticationConfig config)
        {
            foreach (var keyName in KeyNames)
            {
                if (!config.KeyNames.Contains(keyName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
