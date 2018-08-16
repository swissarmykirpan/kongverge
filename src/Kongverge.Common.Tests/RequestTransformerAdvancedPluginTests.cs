using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
using Kongverge.KongPlugin;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class RequestTransformerAdvancedPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new RequestTransformerAdvancedConfig();
            var plugin = new RequestTransformerAdvancedPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new RequestTransformerAdvancedConfig
            {
                id = "test1",
                HttpMethod = "POST",
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Body = Maps("addbody1:value" ),
                    Headers = Maps("addheader1:value" ),
                    QueryString = Maps("addQueryString1:value" )
                },
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Body = Maps("appendbody1:value" ),
                    Headers = Maps("appendHeader1:value" ),
                    QueryString = Maps("appendQueryString1:value" )
                },
                Remove = new RequestTransformerAdvancedTransformRemove
                {
                    Body = new HashSet<string> { "removebody1" },
                    Headers = new HashSet<string> { "removeHeader1" },
                    QueryString = new HashSet<string> { "removeQueryString1" }
                },
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Body = Maps("renamebody1:value" ),
                    Headers = Maps("renameHeader1:value" ),
                    QueryString = Maps("renameQueryString1:value" )
                },
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Body = Maps("replacebody1:value" ),
                    Headers = Maps("replaceHeader1:value" ),
                    QueryString = Maps("replaceQueryString1:value" ),
                    Uri = "bob.com"
                }
            };

            var plugin = new RequestTransformerAdvancedPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new RequestTransformerAdvancedPlugin());
        }

        private static IDictionary<string, string> Maps(params string[] values)
        {
            return ConfigReadExtensions.StringsToMaps(values);
        }
    }
}
