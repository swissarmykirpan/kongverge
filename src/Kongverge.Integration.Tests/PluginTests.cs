using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using Kongverge.Common.DTOs;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public abstract class PluginTests : IClassFixture<KongvergeTestFixture>
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

                await _fixture.AddServiceAndChildren(service);

                var serviceReadFromKong = await _fixture.KongAdminReader.GetService(service.Id);

                var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin(plugin.Name);

                pluginOut.Should()
                    .BeEquivalentTo(plugin, opt => opt
                        .Excluding(p => p.Id)
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

                await _fixture.AddServiceAndChildren(service);

                var serviceReadFromKong = await _fixture.KongAdminReader.GetService(service.Id);

                serviceReadFromKong.Routes.Should().HaveCount(1);

                var pluginOut = serviceReadFromKong.Routes.First().ShouldHaveOnePlugin(plugin.Name);

                pluginOut.Should()
                    .BeEquivalentTo(plugin, opt => opt
                        .Excluding(p => p.Id)
                        .Using<string>(CompareStringsWithoutNull).WhenTypeIs<string>());
            }
        }

        [Fact]
        public async Task ShouldRoundTripGlobalPluginToKong()
        {
            foreach (var plugin in Permutations)
            {
                await _fixture.UpsertPlugin(plugin);
                var plugins = await _fixture.KongAdminReader.GetPlugins();
                var globalPlugins = plugins.Where(x => x.IsGlobal()).ToArray();

                globalPlugins.Single(x => x.Id == plugin.Id).Should()
                    .BeEquivalentTo(plugin, opt => opt
                        .Excluding(p => p.Id)
                        .Using<string>(CompareStringsWithoutNull).WhenTypeIs<string>());

                // We need to delete the globally added plugin because the some plugins for e.g. request-transfomer plugin 
                // cannot be added as a global plugin multiple times.
                await _fixture.DeleteGlobalPlugin(plugin.Id);
            }
        }

        protected abstract IEnumerable<KongPlugin> Permutations { get; }

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
