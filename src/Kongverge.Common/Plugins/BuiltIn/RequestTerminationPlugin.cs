using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{

    public class RequestTerminationPlugin : KongPluginBase<RequestTerminationConfig>
    {
        public RequestTerminationPlugin() : base("request-termination")
        {
        }

        public override string SectionName => "terminate";

        protected override RequestTerminationConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RequestTerminationConfig
            {
                Message = pluginBody.config.ReadString("message"),
                StatusCode = pluginBody.config.ReadLong("status_code")
            };
        }

        protected override PluginBody DoCreatePluginBody(RequestTerminationConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                {"message", target.Message },
                {"status_code", target.StatusCode }
            });
        }
    }
}
