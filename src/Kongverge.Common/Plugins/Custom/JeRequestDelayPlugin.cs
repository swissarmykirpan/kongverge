using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeRequestDelayPlugin : KongPluginBase<JeRequestDelayConfig>
    {
        public JeRequestDelayPlugin() : base("je-request-delay")
        {
        }

        public override string PluginName => "je-request-delay";

        protected override JeRequestDelayConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new JeRequestDelayConfig
            {
                DelayMillis = pluginBody.ReadConfigInt("delay")
            };
        }

        protected override PluginBody DoCreatePluginBody(JeRequestDelayConfig target)
        {
            var config = new Dictionary<string, object>
            {
                { "delay", target.DelayMillis }
            };

            return new PluginBody(PluginName, config);
        }
    }
}