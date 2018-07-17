using Kongverge.Extension;
using System.Collections.Generic;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RateLimitingPlugin : ExtensionBase<RateLimitingConfig>
    {
        public RateLimitingPlugin() : base("rate-limit")
        {
        }

        public override string PluginName => "rate-limiting-advanced";

        public override RateLimitingConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RateLimitingConfig
            {
                Limit = (int)pluginBody.config["limit"],
                Identifier = "ip",
                WindowSize = (int)pluginBody.config["windows_size"]
            };
        }

        public override PluginBody DoCreatePluginBody(RateLimitingConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
                    {
                        {"limit", target.Limit },
                        {"window_size", target.WindowSize },
                        {"identifier", target.Identifier },
                        {"sync_rate", 100 }
                    });
        }
    }
}
