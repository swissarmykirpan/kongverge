using System.Collections.Generic;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn.RequestTransform
{
    public class AdvancedTransformRemove : IAdvancedTransform
    {
        [JsonProperty("headers")]
        public HashSet<string> Headers { get; set; } = new HashSet<string>();

        [JsonProperty("querystring")]
        public HashSet<string> QueryString { get; set; } = new HashSet<string>();

        [JsonProperty("body")]
        public HashSet<string> Body { get; set; } = new HashSet<string>();

        public virtual bool IsExactMatch(IAdvancedTransform other)
        {
            if (other is AdvancedTransformRemove otherConfig)
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
