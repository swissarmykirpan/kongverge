using Kongverge.Extension;

namespace Kongverge.Common.Plugins
{
    public interface IKongPluginCollection
    {
        IKongPluginConfig TranslateToConfig(PluginBody arg);
        PluginBody CreatePluginBody(IKongPluginConfig plugin);
    }
}
