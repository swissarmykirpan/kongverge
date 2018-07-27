using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public class PluginsResponse
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("data")]
        public PluginBody[] Data { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
