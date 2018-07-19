using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedTransformRemove : IRequestTransformerAdvancedNestedConfig
    {
        [JsonProperty("headers")]
        public HashSet<string> Headers { get; set; } = new HashSet<string>();

        [JsonProperty("querystring")]
        public HashSet<string> QueryString { get; set; } = new HashSet<string>();

        [JsonProperty("body")]
        public HashSet<string> Body { get; set; } = new HashSet<string>();

        public bool IsExactMatch(IRequestTransformerAdvancedNestedConfig other)
        {
            if (other is RequestTransformerAdvancedTransformRemove otherConfig)
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
