using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.Plugins.BuiltIn;
using Xunit;

namespace Kongverge.Integration.Tests
{
    public class RequestTransformerPluginTests : IClassFixture<KongvergeTestFixture>
    {
        private readonly KongvergeTestFixture _fixture;

        public RequestTransformerPluginTests(KongvergeTestFixture kongvergeTestFixture)
        {
            _fixture = kongvergeTestFixture;
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

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithAddSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "header1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "param_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "key1:value1" }
                }
            };

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithAppendSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "header1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "param_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "key1:value1" }
                }
            };

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }


        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithRemoveSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Remove = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "header1", "header2", "header3" },
                    QueryString = new HashSet<string> { "param_a", "param_b" },
                    Body = new HashSet<string> { "key1" }
                }
            };

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }


        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithReplaceSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Uri = "/foo/bar",
                    Headers = new HashSet<string> { "header1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "param_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "key1:value1" }
                }
            };

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithRenameSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "header1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "param_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "key1:value1" }
                }
            };

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }
    }
}
