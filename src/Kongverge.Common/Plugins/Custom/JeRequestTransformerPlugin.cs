using Kongverge.Common.Plugins.BuiltIn.RequestTransform;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeRequestTransformerPlugin : RequestTransformerPlugin<JeRequestTransformerConfig>
    {
        public JeRequestTransformerPlugin() : base("je-request-transformer")
        {
        }
    }
}
