using Kongverge.KongPlugin;

namespace Kongverge.Common.DTOs
{
    public class GlobalConfig : ExtendibleKongObject
    {
        protected override PluginBody DoDecoratePluginBody(PluginBody body)
        {
            return body;
        }
    }
}
