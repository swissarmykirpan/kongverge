using System.Collections.Generic;
using Kongverge.Common.Helpers;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RateLimitingPlugin : KongPluginBase<RateLimitingConfig>
    {
        public RateLimitingPlugin() : base("rate-limiting-advanced")
        {
        }

        protected override RateLimitingConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RateLimitingConfig
            {
                Identifier = pluginBody.config.ReadString("identifier").FromJsonString<RateLimitingIdentifier>(),
                Limit = pluginBody.config.ReadInts("limit"),
                WindowSize = pluginBody.config.ReadInts("window_size")
            };
        }

        protected override PluginBody DoCreatePluginBody(RateLimitingConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "limit", target.Limit },
                { "window_size", target.WindowSize },
                { "identifier", target.Identifier.ToJsonString() },
                { "sync_rate", 100 }
            });
        }
    }
}
