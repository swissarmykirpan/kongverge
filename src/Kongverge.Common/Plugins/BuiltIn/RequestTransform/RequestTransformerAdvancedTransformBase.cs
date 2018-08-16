using System.Collections.Generic;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn.RequestTransform
{
    public class RequestTransformerAdvancedTransformBase : IRequestTransformerAdvancedNestedConfig
    {
        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        [JsonProperty("querystring")]
        public IDictionary<string, string> QueryString { get; set; } = new Dictionary<string, string>();

        [JsonProperty("body")]
        public IDictionary<string, string> Body { get; set; } = new Dictionary<string, string>();

        public virtual bool IsExactMatch(IRequestTransformerAdvancedNestedConfig other)
        {
            if (other is RequestTransformerAdvancedTransformBase otherConfig)
            {
                return
                    ConfigReadExtensions.SetsMatch(Headers, otherConfig.Headers) &&
                    ConfigReadExtensions.SetsMatch(QueryString, otherConfig.QueryString) &&
                    ConfigReadExtensions.SetsMatch(Body, otherConfig.Body);
            }

            return false;
        }
    }
}
