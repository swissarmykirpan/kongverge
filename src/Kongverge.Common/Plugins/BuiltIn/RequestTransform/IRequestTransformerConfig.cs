using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins.BuiltIn.RequestTransform
{
    public interface IRequestTransformerConfig : IKongPluginConfig
    {
        string HttpMethod { get; set; }

        AdvancedTransformRemove Remove { get; set; }

        AdvancedTransformReplace Replace { get; set; }

        AdvancedTransform Rename { get; set; }

        AdvancedTransform Add { get; set; }

        AdvancedTransform Append { get; set; }
    }
}
