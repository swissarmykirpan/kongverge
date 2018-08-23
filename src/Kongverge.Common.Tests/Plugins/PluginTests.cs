using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Kongverge.KongPlugin;
using Xunit;

namespace Kongverge.Common.Tests.Plugins
{
    public abstract class PluginTests<TPlugin, TPluginConfig>
        where TPlugin : KongPluginBase<TPluginConfig>, new()
        where TPluginConfig : IKongPluginConfig, new()
    {
        [Fact]
        public void RoundTripFromConfigWithNoFields()
        {
            var configIn = new TPluginConfig();
            var plugin = new TPlugin();

            var configOut = RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromConfigWithAllFields()
        {
            var configIn = new Fixture().Create<TPluginConfig>();
            var plugin = new TPlugin();

            var configOut = RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            var plugin = new TPlugin();
            var bodyIn = new PluginBody(plugin.PluginName, EmptyConfig);

            var bodyOut = RoundTripFromBody(plugin, bodyIn);

            bodyOut.Should().NotBeNull();
            bodyOut.name.Should().Be(bodyIn.name);

            AssertEmptyConfig(bodyOut.config);
        }

        private static TConfig RoundTripFromConfig<TConfig>(KongPluginBase<TConfig> plugin, IKongPluginConfig configIn)
            where TConfig : IKongPluginConfig
        {
            var body = plugin.CreatePluginBody(configIn);

            return (TConfig)plugin.CreateConfigObject(body);
        }

        private static PluginBody RoundTripFromBody<TConfig>(KongPluginBase<TConfig> plugin, PluginBody body)
            where TConfig : IKongPluginConfig
        {
            var config = (TConfig)plugin.CreateConfigObject(body);
            return plugin.CreatePluginBody(config);
        }

        protected virtual Dictionary<string, object> EmptyConfig => new Dictionary<string, object>();

        protected virtual void AssertEmptyConfig(Dictionary<string, object> config) { }
    }
}
