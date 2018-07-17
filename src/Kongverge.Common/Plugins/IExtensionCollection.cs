using Kongverge.Extension;

namespace Kongverge.Common.Plugins
{
    public interface IExtensionCollection
    {
        IKongPluginConfig TranslateToConfig(PluginBody arg);
        PluginBody CreatePluginBody(IKongPluginConfig plugin);
    }
}
