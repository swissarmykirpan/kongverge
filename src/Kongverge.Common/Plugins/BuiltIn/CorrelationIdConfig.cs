using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public enum CorrelationIdGenerator
    {
        UUID,
        Counter,
        Tracker
    }

    public class CorrelationIdConfig : IKongPluginConfig
    {
        [JsonProperty("header")]
        public string Header { get; set; }
        [JsonProperty("template")]
        public CorrelationIdGenerator Template { get; set; }
        public bool EchoDownstream { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is CorrelationIdConfig otherConfig)
            {
                return Header == otherConfig.Header
                       && Template == otherConfig.Template
                       && EchoDownstream == otherConfig.EchoDownstream;
            }

            return false;
        }
    }
}
