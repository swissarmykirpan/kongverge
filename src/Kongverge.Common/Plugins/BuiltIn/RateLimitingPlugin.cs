using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RateLimitingPlugin : KongPluginBase<RateLimitingConfig>
    {
        public RateLimitingPlugin() : base("rate-limit")
        {
        }

        public override string[] PluginNames => new []{"rate-limiting-advanced"};

        protected override RateLimitingConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RateLimitingConfig
            {
                Identifier = pluginBody.ReadConfigString("identifier"),
                Limit = pluginBody.ReadConfigInt("limit"),
                WindowSize = pluginBody.ReadConfigInt("window_size")
            };
        }

        protected override PluginBody DoCreatePluginBody(RateLimitingConfig target)
        {
            return new PluginBody(PluginNames[0], new Dictionary<string, object>
                    {
                        {"limit", target.Limit },
                        {"window_size", target.WindowSize },
                        {"identifier", target.Identifier },
                        {"sync_rate", 100 }
                    });
        }
    }
}
