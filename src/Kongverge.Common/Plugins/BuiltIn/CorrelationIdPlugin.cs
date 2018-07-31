using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class CorrelationIdPlugin : KongPluginBase<CorrelationIdConfig>
    {
        public CorrelationIdPlugin() : base("correlation")
        {
        }

        public override string PluginName => "correlation-id";

        protected override CorrelationIdConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new CorrelationIdConfig
            {
                EchoDownstream = pluginBody.ReadConfigBool("echo_downstream"),
                Template = pluginBody.ReadConfigString("generator"),
                Header = pluginBody.ReadConfigString("header_name")
            };
        }

        protected override PluginBody DoCreatePluginBody(CorrelationIdConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                {"echo_downstream", target.EchoDownstream },
                { "generator", target.Template },
                { "header_name", target.Header }
            });
        }
    }
}
