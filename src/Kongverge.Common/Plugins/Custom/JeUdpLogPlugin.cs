using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeUdpLogPlugin : KongPluginBase<JeUdpLogConfig>
    {
        public JeUdpLogPlugin() : base("je-udp-log")
        {
        }

        protected override JeUdpLogConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new JeUdpLogConfig
            {
                Timeout = pluginBody.config.ReadInt("timeout"),
                Host = pluginBody.config.ReadString("host"),
                Port = pluginBody.config.ReadInt("port"),
                LogBody = pluginBody.config.ReadBool("log_body"),
                MaxBodySize = pluginBody.config.ReadInt("max_body_size")
            };
        }

        protected override PluginBody DoCreatePluginBody(JeUdpLogConfig target)
        {
            var config = new Dictionary<string, object>
            {
                { "timeout", target.Timeout },
                { "host", target.Host },
                { "port", target.Port},
                { "log_body", target.LogBody},
                { "max_body_size", target.MaxBodySize}
            };
            return new PluginBody(PluginName, config);
        }
    }
}
