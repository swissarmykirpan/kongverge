using Kongverge.Common.Plugins.BuiltIn;

namespace Kongverge.Common.Plugins.Custom
{
    public class JeRequestTransformerPlugin : BaseRequestTransformerPlugin<JeRequestTransformerConfig>
    {
        public JeRequestTransformerPlugin() : base("je-request-transformer")
        {
        }

        public override string PluginName => "je-request-transformer";
    }
}
