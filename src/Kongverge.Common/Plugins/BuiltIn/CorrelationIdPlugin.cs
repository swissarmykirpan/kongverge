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

        public override CorrelationIdConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new CorrelationIdConfig
            {
                EchoDownstream = (bool)pluginBody.config["echo_downstream"],
                Template = pluginBody.config["generator"].ToString(),
                Header = pluginBody.config["header_name"].ToString()
            };
        }

        public override PluginBody DoCreatePluginBody(CorrelationIdConfig target)
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
