using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class BuiltInPluginTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public BuiltInPluginTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
        }

        [Fact]
        public async Task DefaultServiceHasNoPlugins()
        {
            var service = new ServiceBuilder().AddDefaultTestService().Build();
            var kongServiceAdded = await _fixture.AddServiceAndPlugins(service);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            serviceReadFromKong.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().NotBeNull();
            serviceReadFromKong.Plugins.Should().BeEmpty();
        }

        [Fact]
        public async Task ServiceCanHaveCorrelationIdPlugin()
        {
            var plugin = new CorrelationIdConfig
            {
                Header = "test1"
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<CorrelationIdConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveKeyAuthenticationPlugin()
        {
            var plugin = new KeyAuthenticationConfig();

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<KeyAuthenticationConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRateLimitingPlugin()
        {
            var plugin = new RateLimitingConfig
            {
                Identifier = "consumer",
                Limit = new [] { 123 },
                WindowSize = new [] { 3455 }
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<RateLimitingConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTerminationPlugin()
        {
            var plugin = new RequestTerminationConfig
            {
                StatusCode = 501,
                Message = "test term"
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<RequestTerminationConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithMinimalConfig()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Uri = "/foo/bar",
                }
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<RequestTransformerAdvancedConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }


       [Fact(Skip = "This does not work yet. Will be next.")]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithFullConfig()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Uri = "/foo/bar",
                    Headers = new HashSet<string> { "foo1", "bar2" },
                    QueryString = new HashSet<string> { "aa", "bb" },
                    Body = new HashSet<string> { "11", "22" }
                },
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "foo", "bar" },
                    QueryString = new HashSet<string> { "a", "b" },
                    Body = new HashSet<string> { "1", "2" }
                },
                Remove =new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "Remove_a", "Remove_b" },
                    QueryString = new HashSet<string> { "Remove_c", "Remove_d" },
                    Body = new HashSet<string> { "Remove_e", "Remove_f" }
                },
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "Rename_a", "Rename_b" },
                    QueryString = new HashSet<string> { "Rename_c", "Rename_d" },
                    Body = new HashSet<string> { "Rename_e", "Rename_f" }
                },
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "Append_a", "Append_b" },
                    QueryString = new HashSet<string> { "Append_c", "Append_d" },
                    Body = new HashSet<string> { "Append_e", "Append_f" }
                }
            };

            var kongServiceAdded = await _fixture.AttachPluginToService(plugin);

            var serviceReadFromKong = await _fixture.KongAdminReader.GetService(kongServiceAdded.Id);

            var pluginOut = serviceReadFromKong.ShouldHaveOnePlugin<RequestTransformerAdvancedConfig>();

            plugin.id = pluginOut.id;
            pluginOut.Should().BeEquivalentTo(plugin);
        }

    }
}
