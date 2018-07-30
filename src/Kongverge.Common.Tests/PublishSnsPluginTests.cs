using FluentAssertions;
using Kongverge.Common.Plugins.Custom;
using Kongverge.KongPlugin;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class PublishSnsPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new PublishSnsConfig();
            var plugin = new PublishSnsPlugin();

            var configOut = RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
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

            var configOut = RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        private static TConfig RoundTripFromConfig<TConfig>(
            KongPluginBase<TConfig> plugin, PublishSnsConfig configIn)
            where TConfig : IKongPluginConfig
        {
            var body = plugin.CreatePluginBody(configIn);

            return (TConfig)plugin.CreateConfigObject(body);
        }
    }
}
