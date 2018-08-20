using System.Collections.Generic;
using Kongverge.Common.Helpers;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class CorrelationIdPlugin : KongPluginBase<CorrelationIdConfig>
    {
        public CorrelationIdPlugin() : base("correlation-id")
        {
        }

        protected override CorrelationIdConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new CorrelationIdConfig
            {
                Generator = pluginBody.config.ReadString("generator").FromJsonString<CorrelationIdGenerator>(),
                EchoDownstream = pluginBody.config.ReadBool("echo_downstream"),
                HeaderName = pluginBody.config.ReadString("header_name")
            };
        }

        protected override PluginBody DoCreatePluginBody(CorrelationIdConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "echo_downstream", target.EchoDownstream },
                { "generator", target.Generator.ToJsonString() },
                { "header_name", target.HeaderName }
            });
        }
    }
}
