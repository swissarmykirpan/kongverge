using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedTransformReplace : RequestTransformerAdvancedTransformBase
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        public override bool IsExactMatch(IRequestTransformerAdvancedNestedConfig other)
        {
            var isMatch = base.IsExactMatch(other);

            if (!isMatch)
            {
                return false;
            }

            if (other is RequestTransformerAdvancedTransformReplace otherConfig)
            {
                return Uri == otherConfig.Uri;
            }

            return false;
        }
    }
}
