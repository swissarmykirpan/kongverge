using System.Net;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTerminationConfig : IKongPluginConfig
    {
        [JsonProperty("status_code")]
        public HttpStatusCode StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is RequestTerminationConfig otherConfig)
            {
                return Message == otherConfig.Message &&
                       StatusCode == otherConfig.StatusCode;
            }

            return false;
        }
    }
}
