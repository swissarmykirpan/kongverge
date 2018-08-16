using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.Transform
{
    public interface IRequestTransformerConfig : IKongPluginConfig
    {
        string HttpMethod { get; set; }

        RequestTransformerAdvancedTransformBase Remove { get; set; }

        RequestTransformerAdvancedTransformReplace Replace { get; set; }

        RequestTransformerAdvancedTransformBase Rename { get; set; }

        RequestTransformerAdvancedTransformBase Add { get; set; }

        RequestTransformerAdvancedTransformBase Append { get; set; }
    }
}
