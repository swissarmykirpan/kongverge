using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn.RequestTransform
{
    public class AdvancedTransformReplace : AdvancedTransform
    {
        [JsonProperty("uri")]
        public string Uri { get; set; }

        public override bool IsExactMatch(IAdvancedTransform other)
        {
            var isMatch = base.IsExactMatch(other);

            if (!isMatch)
            {
                return false;
            }

            if (other is AdvancedTransformReplace otherConfig)
            {
                return Uri == otherConfig.Uri;
            }

            return false;
        }
    }
}
