using Kongverge.Common.Plugins.Custom;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Tests
{
    public static class PluginHelpers
    {
        public static TConfig RoundTripFromConfig<TConfig>(
            KongPluginBase<TConfig> plugin, IKongPluginConfig configIn)
            where TConfig : IKongPluginConfig
        {
            var body = plugin.CreatePluginBody(configIn);

            return (TConfig)plugin.CreateConfigObject(body);
        }

        public static PluginBody RoundTripFromBody<TConfig>(
            KongPluginBase<TConfig> plugin, PluginBody body)
            where TConfig : IKongPluginConfig
        {
            var config = (TConfig)plugin.CreateConfigObject(body);
            return plugin.CreatePluginBody(config);
        }
    }
}
