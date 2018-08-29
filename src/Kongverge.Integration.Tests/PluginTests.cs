using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
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
        public async Task ShouldRoundTripServicePluginToKong()
        {
            foreach (var plugin in Permutations)
            {
                var service = _fixture.ServiceBuilder
                    .CreateDefaultTestService()
                    .WithPlugin(plugin)
                    .Build();

                var kongServiceAdded = await _fixture.AddServiceAndChildren(service);

                var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

                var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<TPluginConfig>();

                pluginOut.Should()
                    .BeEquivalentTo(plugin, opt => opt
                        .Excluding(p => p.id)
                        .Using<string>(CompareStringsWithoutNull).WhenTypeIs<string>());
            }
        }

        [Fact]
        public async Task ShouldRoundTripRoutePluginToKong()
        {
            foreach (var plugin in Permutations)
            {
                var service = _fixture.ServiceBuilder
                    .CreateDefaultTestService()
                    .WithRoutePaths("/path/one", "/another/route")
                    .WithRoutePlugin(plugin)
                    .Build();

                var kongServiceAdded = await _fixture.AddServiceAndChildren(service);

                var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

                serviceReadFromKong.Routes.Should().HaveCount(1);

                var pluginOut = serviceReadFromKong.Routes.First().ShouldHaveOnePlugin<TPluginConfig>();

                pluginOut.Should()
                    .BeEquivalentTo(plugin, opt => opt
                        .Excluding(p => p.id)
                        .Using<string>(CompareStringsWithoutNull).WhenTypeIs<string>());
            }
        }

        [Fact]
        public async Task ShouldRoundTripGlobalPluginToKong()
        {
            foreach (var plugin in Permutations)
            {
                var pluginBody = _fixture.PluginCollection.CreatePluginBody(plugin);
                var pluginOut = await _fixture.ShouldUpsertPlugin(pluginBody);
                var globalPlugins = await _fixture.KongAdminReader.GetGlobalConfig();

                globalPlugins.Plugins.Single(x => x.id == pluginOut.Id).Should()
                    .BeEquivalentTo(plugin, opt => opt
                        .Excluding(p => p.id)
                        .Using<string>(CompareStringsWithoutNull).WhenTypeIs<string>());

                // We need to delete the globally added plugin because the some plugins for e.g. request-transfomer plugin 
                // cannot be added as a global plugin multiple times.
                await _fixture.DeleteGlobalPlugin(pluginOut.Id);
            }
        }

        protected virtual IEnumerable<TPluginConfig> Permutations
        {
            get
            {
                yield return new Fixture().Create<TPluginConfig>();
            }
        }

        /// <summary>
        /// Treat null string and empty string as equivalent
        /// </summary>
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
