using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Kongverge.KongPlugin;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class PublishSnsPluginTests
    {
        [Fact]
        public void RoundTripFromConfigWithNoFields()
        {
            var configIn = new PublishSnsConfig();
            var plugin = new PublishSnsPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromConfigWithAllFields()
        {
            var configIn = new PublishSnsConfig
            {
                AccountId = 1234,
                AwsKey = "aKey",
                AwsSecret = "shhh",
                AwsRegion = "somehwere",
                Timeout = 1234,
                TopicName = "testTopic",
                KeepAlive = 3,
                ConsulUrl = "/con/sul",
                Environment = "qaxx",
                id = "test1"
            };

            var plugin = new PublishSnsPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            var plugin = new PublishSnsPlugin();
            var bodyIn = new PluginBody(plugin.PluginName, new Dictionary<string, object>());

            var bodyOut = PluginHelpers.RoundTripFromBody(plugin, bodyIn);

            bodyOut.Should().NotBeNull();
            bodyOut.name.Should().Be(bodyIn.name);
        }
    }
}
