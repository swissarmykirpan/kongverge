using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeJustSayingDefaultsPlugin : KongPluginBase<JeJustSayingDefaultsConfig>
    {
        public JeJustSayingDefaultsPlugin() : base("je-tcp-log")
        {
        }

        public override string PluginName => "je-tcp-log";

        protected override JeJustSayingDefaultsConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new JeJustSayingDefaultsConfig
            {
                RaisingComponent = pluginBody.ReadConfigString("raisingcomponent"),
                Tenant = pluginBody.ReadConfigString("tenant")
            };
        }

        protected override PluginBody DoCreatePluginBody(JeJustSayingDefaultsConfig target)
        {
            var config = new Dictionary<string, object>
            {
                { "raisingcomponent", target.RaisingComponent },
                { "tenant", target.Tenant}
            };

            return new PluginBody(PluginName, config);
        }
    }
}
