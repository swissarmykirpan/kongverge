using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeTcpLogPlugin : KongPluginBase<JeTcpLogConfig>
    {
        public JeTcpLogPlugin() : base("je-tcp-log")
        {
        }

        protected override JeTcpLogConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new JeTcpLogConfig
            {
                Timeout = pluginBody.config.ReadInt("timeout"),
                KeepAlive = pluginBody.config.ReadInt("keepalive"),
                Host = pluginBody.config.ReadString("host"),
                Port = pluginBody.config.ReadInt("port"),
                LogBody = pluginBody.config.ReadBool("log_body"),
                MaxBodySize = pluginBody.config.ReadInt("max_body_size")
            };
        }

        protected override PluginBody DoCreatePluginBody(JeTcpLogConfig target)
        {
            var config = new Dictionary<string, object>
            {
                { "timeout", target.Timeout },
                { "host", target.Host },
                { "port", target.Port},
                { "log_body", target.LogBody },
                { "max_body_size", target.MaxBodySize },
                { "keepalive" , target.KeepAlive }
            };

            return new PluginBody(PluginName, config);
        }
    }
}
