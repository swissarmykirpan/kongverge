using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.Custom
{
    public class JELoggingConfig : IKongPluginConfig
    {
        public int Timeout { get; set; }

        public int KeepAlive { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("log_body")]
        public bool LogBody { get; set; }

        [JsonProperty("max_body_size")]
        public int MaxBodySize { get; set; } = 65536;

        [JsonProperty("protocol")]
        public string Protocol { get; set; } = "udp";



        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is JELoggingConfig otherConfig)
            {
                return
                    Timeout == otherConfig.Timeout &&
                    KeepAlive == otherConfig.KeepAlive &&
                    Host== otherConfig.Host &&
                    LogBody== otherConfig.LogBody &&
                    MaxBodySize == otherConfig.MaxBodySize &&
                    Port== otherConfig.Port;
            }

            return false;
        }

        public string id { get; set; }

        public bool ChangeRequiresReplacing(IKongPluginConfig other)
        {
            if (other is JELoggingConfig otherConfig)
            {
                return Protocol != otherConfig.Protocol;
            }

            return false;
        }
    }
}
