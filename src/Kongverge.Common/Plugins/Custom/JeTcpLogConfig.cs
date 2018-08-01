using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeTcpLogConfig : IKongPluginConfig
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
        
        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is JeTcpLogConfig otherConfig)
            {
                return
                    Timeout == otherConfig.Timeout &&
                    KeepAlive == otherConfig.KeepAlive &&
                    Host == otherConfig.Host &&
                    Port == otherConfig.Port &&
                    LogBody == otherConfig.LogBody &&
                    MaxBodySize == otherConfig.MaxBodySize;
            }
            return false;
        }
        public string id { get; set; }
    }
}
