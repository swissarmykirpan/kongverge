using System.Collections.Generic;
using System.Threading.Tasks;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
using Kongverge.KongPlugin;
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

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithAddSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = Maps("header1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps("param_a:valueb", "param_b:value_d" ),
                    Body = Maps("key1:value1" )
                }
            };

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithAppendSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = Maps("header1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps("param_a:valueb", "param_b:value_d" ),
                    Body = Maps("key1:value1" )
                }
            };

            await _fixture.ShouldRoundTripPluginToKong(plugin);
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

            await _fixture.ShouldRoundTripPluginToKong(plugin);
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
                    Headers = Maps("header1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps("param_a:valueb", "param_b:value_d" ),
                    Body = Maps("key1:value1" )
                }
            };

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithRenameSection()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = Maps("header1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps("param_a:valueb", "param_b:value_d" ),
                    Body = Maps("key1:value1" )
                }
            };

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }

        [Fact]
        public async Task ServiceCanHaveRequestTransformerAdvancedPluginWithAllFieldsSet()
        {
            var plugin = new RequestTransformerAdvancedConfig
            {
                HttpMethod = "GET",
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Headers = Maps("aheader1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps("aparam_a:valueb", "param_b:value_d" ),
                    Body = Maps("akey1:value1" )
                },
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Headers = Maps("bheader1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps("bparam_a:valueb", "param_b:value_d" ),
                    Body = Maps("bkey1:value1" )
                },
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Headers = Maps("cheader1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps("cparam_a:valueb", "param_b:value_d" ),
                    Body = Maps("ckey1:value1" )
                },
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Uri = "/foo/bar",
                    Headers = Maps( "dheader1:value1", "header2:Value2", "header3:value3" ),
                    QueryString = Maps( "dparam_a:valueb", "param_b:value_d" ),
                    Body = Maps( "dkey1:value1" )
                },
                Remove = new RequestTransformerAdvancedTransformRemove
                {
                    Headers = new HashSet<string> { "eheader1", "header2", "header3" },
                    QueryString = new HashSet<string> { "eparam_a", "param_b" },
                    Body = new HashSet<string> { "ekey1" }
                }
            };

            await _fixture.ShouldRoundTripPluginToKong(plugin);
        }

        private static IDictionary<string, string> Maps(params string[] values)
        {
            return ConfigReadExtensions.StringsToMaps(values);
        }
    }
}
