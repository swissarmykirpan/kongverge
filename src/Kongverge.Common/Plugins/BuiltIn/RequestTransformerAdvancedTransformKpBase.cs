using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedTransformKpBase : IRequestTransformerAdvancedNestedConfig
    {
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        [JsonProperty("querystring")]
        public Dictionary<string, string> QueryString { get; set; } = new Dictionary<string, string>();

        [JsonProperty("body")]
        public Dictionary<string, string> Body { get; set; } = new Dictionary<string, string>();

        public virtual bool IsExactMatch(IRequestTransformerAdvancedNestedConfig other)
        {
            if (other is RequestTransformerAdvancedTransformKpBase otherConfig)
            {
                return !Headers.Except(otherConfig.Headers)
                           .Concat(otherConfig.Headers.Except(Headers)).Any()

                       && !QueryString.Except(otherConfig.QueryString)
                           .Concat(otherConfig.QueryString.Except(QueryString)).Any()

                       && !Body.Except(otherConfig.Body)
                           .Concat(otherConfig.Body.Except(Body)).Any();
            }

            return false;
        }
    }
}
