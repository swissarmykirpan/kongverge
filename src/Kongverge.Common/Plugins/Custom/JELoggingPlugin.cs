using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class JELoggingPlugin : KongPluginBase<JELoggingConfig>
    {
        private readonly string[] _pluginLookup = {"je-udp-log" , "je-tcp-log"};

        public JELoggingPlugin() : base("logging")
        {
        }

        public override string[] PluginNames => _pluginLookup;

        protected override JELoggingConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new JELoggingConfig
            {
                Timeout = pluginBody.ReadConfigInt("timeout"),
                KeepAlive = pluginBody.ReadConfigInt("keepalive"),
                Host = pluginBody.ReadConfigString("host"),
                Port = pluginBody.ReadConfigInt("port"),
                LogBody = pluginBody.ReadConfigBool("log_body"),
                MaxBodySize = pluginBody.ReadConfigInt("max_body_size"),
                Protocol = pluginBody.name.Contains("udp")? "udp": "tcp"
            };
        }

        protected override PluginBody DoCreatePluginBody(JELoggingConfig target)
        {
            var config = new Dictionary<string, object>
            {
                { "timeout", target.Timeout },
                { "host", target.Host },
                { "port", target.Port},
                { "log_body", target.LogBody},
                { "max_body_size", target.MaxBodySize}
            };

            if (target.Protocol.ToLower(CultureInfo.InvariantCulture) == "tcp")
            {
                config.Add("keepalive", target.KeepAlive);
            }

            var protocol = _pluginLookup[0];
            if (target.Protocol.Contains("tcp"))
            {
                protocol = _pluginLookup[1];
            }
            return new PluginBody(protocol, config);
        }
    }
}
