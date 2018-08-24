using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeJustSayingDefaultsPlugin : KongPluginBase<JeJustSayingDefaultsConfig>
    {
        public JeJustSayingDefaultsPlugin() : base("je-justsaying-defaults")
        {
        }

        protected override JeJustSayingDefaultsConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new JeJustSayingDefaultsConfig
            {
                RaisingComponent = pluginBody.config.ReadString("raisingcomponent"),
                Tenant = pluginBody.config.ReadString("tenant")
            };
        }

        protected override PluginBody DoCreatePluginBody(JeJustSayingDefaultsConfig target)
        {
            var config = new Dictionary<string, object>
            {
                { "raisingcomponent", target.RaisingComponent },
                { "tenant", target.Tenant }
            };

            return new PluginBody(PluginName, config);
        }
    }
}
