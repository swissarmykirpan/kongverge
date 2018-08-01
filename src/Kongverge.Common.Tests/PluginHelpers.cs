using System.Collections.Generic;
using FluentAssertions;
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

        public static PluginBody RoundTripFromBodyTest<TConfig>(
            KongPluginBase<TConfig> plugin,
            Dictionary<string, object> configData = null)
            where TConfig : IKongPluginConfig
        {
            configData = configData ?? new Dictionary<string, object>();
            var bodyIn = new PluginBody(plugin.PluginNames[0], configData);

            var bodyOut = RoundTripFromBody(plugin, bodyIn);

            bodyOut.Should().NotBeNull();
            bodyOut.name.Should().Be(bodyIn.name);
            return bodyOut;
        }
    }
}
