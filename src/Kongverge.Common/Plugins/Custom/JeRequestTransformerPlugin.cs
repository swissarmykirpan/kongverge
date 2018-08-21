using Kongverge.Common.Plugins.BuiltIn;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeRequestTransformerPlugin : BaseRequestTransformerPlugin<JeRequestTransformerConfig>
    {
        public JeRequestTransformerPlugin() : base("je-request-transformer")
        {
        }
    }
}
