using Kongverge.Extension;
using System.Collections.Generic;

namespace Kongverge.Common.Plugins.BuiltIn
{

    public class RequestTerminationPlugin : KongPluginBase<RequestTerminationConfig>
    {
        public RequestTerminationPlugin() : base("terminate")
        {
        }

        public override string PluginName => "request-termination";

        public override RequestTerminationConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RequestTerminationConfig
            {
                Message = pluginBody.config["message"].ToString(),
                StatusCode = (long)pluginBody.config["status_code"]
            };
        }

        public override PluginBody DoCreatePluginBody(RequestTerminationConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                {"message", target.Message },
                {"status_code", target.StatusCode }
            });
        }
    }
}
