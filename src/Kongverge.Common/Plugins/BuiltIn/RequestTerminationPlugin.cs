using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{

    public class RequestTerminationPlugin : KongPluginBase<RequestTerminationConfig>
    {
        public RequestTerminationPlugin() : base("terminate")
        {
        }

        public override string[] PluginNames => new[]{"request-termination"};

        protected override RequestTerminationConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RequestTerminationConfig
            {
                Message = pluginBody.config["message"].ToString(),
                StatusCode = (long)pluginBody.config["status_code"]
            };
        }

        protected override PluginBody DoCreatePluginBody(RequestTerminationConfig target)
        {
            return new PluginBody(PluginNames[0], new Dictionary<string, object>
            {
                {"message", target.Message },
                {"status_code", target.StatusCode }
            });
        }
    }
}
