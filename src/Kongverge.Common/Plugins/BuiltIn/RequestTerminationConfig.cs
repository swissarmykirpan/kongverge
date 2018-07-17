using Kongverge.Extension;
using Newtonsoft.Json;
using System;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTerminationConfig : IKongPluginConfig
    {
        [JsonProperty("status_code")]
        public long StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        public bool IsExactmatch(IKongPluginConfig other)
        {
            if (other is RequestTerminationConfig othRequestTermination)
            {
                return Message == othRequestTermination.Message
                       && StatusCode == othRequestTermination.StatusCode;
            }

            return false;
        }
    }
}
