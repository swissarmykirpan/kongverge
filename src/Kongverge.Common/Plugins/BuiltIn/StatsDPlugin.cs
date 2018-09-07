using System.Collections.Generic;
using Kongverge.KongPlugin;
using Newtonsoft.Json.Linq;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class StatsDPlugin : KongPluginBase<StatsDConfig>
    {
        public StatsDPlugin() : base("statsd")
        {
        }

        protected override StatsDConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new StatsDConfig
            {
                Host = pluginBody.config.ReadString("host"),
                Port = pluginBody.config.ReadInt("port"),
                Metrics = pluginBody.config.ReadObjectSet<StatsDMetricConfig>("metrics"),
                Prefix = pluginBody.config.ReadString("prefix")
            };
        }

        protected override PluginBody DoCreatePluginBody(StatsDConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "host", target.Host },
                { "port", target.Port },
                { "metrics", target.Metrics.ToJArray() },
                { "prefix", target.Prefix }
            });
        }
    }
}
