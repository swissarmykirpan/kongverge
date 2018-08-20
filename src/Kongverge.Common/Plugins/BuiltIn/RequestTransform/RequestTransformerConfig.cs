using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.Plugins.BuiltIn.RequestTransform
{
    public abstract class RequestTransformerConfig : IRequestTransformerConfig
    {
        [JsonProperty("http_method")]
        public string HttpMethod { get; set; }

        [JsonProperty("remove")]
        public AdvancedTransformRemove Remove { get; set; } = new AdvancedTransformRemove();

        [JsonProperty("replace")]
        public AdvancedTransformReplace Replace { get; set; } = new AdvancedTransformReplace();

        [JsonProperty("rename")]
        public AdvancedTransform Rename { get; set; } = new AdvancedTransform();

        [JsonProperty("add")]
        public AdvancedTransform Add { get; set; } = new AdvancedTransform();

        [JsonProperty("append")]
        public AdvancedTransform Append { get; set; } = new AdvancedTransform();

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            if (other is IRequestTransformerConfig otherConfig)
            {
                return otherConfig.HttpMethod == HttpMethod
                       && IsNestedConfigMatch(Remove, otherConfig.Remove)
                       && IsNestedConfigMatch(Replace, otherConfig.Replace)
                       && IsNestedConfigMatch(Rename, otherConfig.Rename)
                       && IsNestedConfigMatch(Add, otherConfig.Add)
                       && IsNestedConfigMatch(Append, otherConfig.Append);
            }

            return false;
        }

        private static bool IsNestedConfigMatch(
            IAdvancedTransform transform,
            IAdvancedTransform otherTransform)
        {
            if (transform == null && otherTransform == null)
            {
                return true;
            }

            if (transform != null)
            {
                return transform.IsExactMatch(otherTransform);
            }

            return false;
        }
    }
}
