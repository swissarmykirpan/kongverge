using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
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
                Remove = new RequestTransformerAdvancedTransformRemove
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

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithAllFieldsSet()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "aheader1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "aparam_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "akey1:value1" }
                },
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "bheader1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "bparam_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "bkey1:value1" }
                },
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = new HashSet<string> { "cheader1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "cparam_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "ckey1:value1" }
                },
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Uri = "/foo/bar",
                    Headers = new HashSet<string> { "dheader1:value1", "header2:Value2", "header3:value3" },
                    QueryString = new HashSet<string> { "dparam_a:valueb", "param_b:value_d" },
                    Body = new HashSet<string> { "dkey1:value1" }
                },
                Remove = new RequestTransformerAdvancedTransformRemove
                {
                    Headers = new HashSet<string> { "eheader1", "header2", "header3" },
                    QueryString = new HashSet<string> { "eparam_a", "param_b" },
                    Body = new HashSet<string> { "ekey1" }
                }
            };

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }
    }
}
