using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using Kongverge.KongPlugin;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public abstract class PluginTests<TPluginConfig> : IClassFixture<KongvergeTestFixture>
        where TPluginConfig : IKongPluginConfig, new()
    {
        private readonly KongvergeTestFixture _fixture;

        protected PluginTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task ShouldRoundTripPluginToKong()
        {
            foreach (var plugin in Permutations)
            {
                var service = new ServiceBuilder()
                    .AddDefaultTestService()
                    .WithPlugin(plugin)
                    .Build();

                var kongServiceAdded = await _fixture.AddServiceAndPlugins(service);

                var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

                var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<TPluginConfig>();

                pluginOut.Should()
                    .BeEquivalentTo(plugin, opt => opt
                        .Excluding(p => p.id)
                        .Using<string>(CompareStringsWithoutNull).WhenTypeIs<string>());
            }
        }

        protected virtual IEnumerable<TPluginConfig> Permutations
        {
            get
            {
                yield return Builder<TPluginConfig>.CreateNew().Build();
            }
        }

        /// <summary>
        /// Treat null string and empty string as equivalent
        /// </summary>
        /// <param name="ctx"></param>
        private static void CompareStringsWithoutNull(IAssertionContext<string> ctx)
        {
            var equal = (ctx.Subject ?? string.Empty).Equals(ctx.Expectation ?? string.Empty);

            Execute.Assertion
                .BecauseOf(ctx.Because, ctx.BecauseArgs)
                .ForCondition(equal)
                .FailWith("Expected {context:string} to be {0}{reason}, but found {1}", ctx.Subject, ctx.Expectation);
        }
    }
}
