using System.Collections.Generic;
using System.Net;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn
{

    public class RequestTerminationPlugin : KongPluginBase<RequestTerminationConfig>
    {
        public RequestTerminationPlugin() : base("request-termination")
        {
        }

        protected override RequestTerminationConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new RequestTerminationConfig
            {
                Message = pluginBody.config.ReadString("message"),
                StatusCode = (HttpStatusCode)pluginBody.config.ReadInt("status_code")
            };
        }

        protected override PluginBody DoCreatePluginBody(RequestTerminationConfig target)
        {
            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "message", target.Message },
                { "status_code", (int)target.StatusCode }
            });
        }
    }
}
