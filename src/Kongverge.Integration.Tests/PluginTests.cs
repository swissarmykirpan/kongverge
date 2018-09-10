using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.DTOs;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class PluginTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public PluginTests(KongvergeTestFixture kongvergeTestFixture)
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

                var plugins = await _fixture.KongAdminReader.GetPlugins();
                var pluginOut = plugins.SingleOrDefault(x => x.ServiceId == service.Id);

                plugin.Equals(pluginOut).Should().BeTrue();
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

                var plugins = await _fixture.KongAdminReader.GetPlugins();
                var pluginOut = plugins.SingleOrDefault(x => x.RouteId == service.Routes[0].Id);

                plugin.Equals(pluginOut).Should().BeTrue();
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

                globalPlugins.Single(x => x.Id == plugin.Id).Equals(plugin).Should().BeTrue();

                // We need to delete the globally added plugin because the some plugins for e.g. request-transfomer plugin 
                // cannot be added as a global plugin multiple times.
                await _fixture.DeleteGlobalPlugin(plugin.Id);
            }
        }

        protected IEnumerable<KongPlugin> Permutations
        {
            get
            {
                // TODO: Figure out a way to generate valid plugin config for all supported plugins
                yield return new KongPlugin
                {
                    Name = "je-request-delay",
                    Config = new Dictionary<string, object>
                    {
                        { "delay", 1000 }
                    }
                };
            }
        }
    }
}
