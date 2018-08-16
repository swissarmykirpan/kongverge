using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.Transform
{
    public class RequestTransformerAdvancedTransformBase : IRequestTransformerAdvancedNestedConfig
    {
        [JsonProperty("headers")]
        public HashSet<string> Headers { get; set; } = new HashSet<string>();

        [JsonProperty("querystring")]
        public HashSet<string> QueryString { get; set; } = new HashSet<string>();

        [JsonProperty("body")]
        public HashSet<string> Body { get; set; } = new HashSet<string>();

        public virtual bool IsExactMatch(IRequestTransformerAdvancedNestedConfig other)
        {
            if (other is RequestTransformerAdvancedTransformBase otherConfig)
            {
                return
                    SetsMatch(Headers, otherConfig.Headers) &&
                    SetsMatch(QueryString, otherConfig.QueryString) &&
                    SetsMatch(Body, otherConfig.Body);
            }

            return false;
        }

        private static bool SetsMatch<T>(ICollection<T> a, ICollection<T> b)
        {
            if (a.Count != b.Count)
            {
                return false;
            }

            return !a.Except(b).Concat(b.Except(a)).Any();
        }
    }
}
