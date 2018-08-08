namespace Kongverge.Common.Plugins.BuiltIn
{
    public class RequestTransformerAdvancedPlugin : BaseRequestTransformerPlugin<RequestTransformerAdvancedConfig>
    {
        public RequestTransformerAdvancedPlugin() : base("request-transformer-advanced")
        {
        }

        public override string PluginName => "request-transformer-advanced";
    }
}
