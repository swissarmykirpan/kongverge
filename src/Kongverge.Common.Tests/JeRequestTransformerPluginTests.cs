using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
using Kongverge.Common.Plugins.Custom;
using Kongverge.KongPlugin;
using Xunit;

namespace Kongverge.Common.Tests
{
    public class JeRequestTransformerPluginTests
    {
        [Fact]
        public void TestRoundTripStartingAtConfigWithNoFields()
        {
            var configIn = new JeRequestTransformerConfig();
            var plugin = new JeRequestTransformerPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAddSection()
        {
            var configIn = new JeRequestTransformerConfig
            {
                id = "test1",
                HttpMethod = "POST",
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Body = Maps("addbody1:value","addbody2:value2"),
                    Headers = Maps("addheader1:value"),
                    QueryString = Maps("addQueryString1:value")
                }
            };

            var plugin = new JeRequestTransformerPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithRemoveSection()
        {
            var configIn = new JeRequestTransformerConfig
            {
                id = "test1",
                HttpMethod = "POST",
                Remove = new RequestTransformerAdvancedTransformRemove
                {
                    Body = new HashSet<string> { "removebody1" },
                    Headers = new HashSet<string> { "removeHeader1" },
                    QueryString = new HashSet<string> { "removeQueryString1" }
                }
            };

            var plugin = new JeRequestTransformerPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void TestRoundTripStartingAtConfigWithAllFields()
        {
            var configIn = new JeRequestTransformerConfig
            {
                id = "test1",
                HttpMethod = "POST",
                Add = new RequestTransformerAdvancedTransformBase
                {
                    Body = Maps("addbody1:value", "addbody2:value2"),
                    Headers = Maps("addheader1:value"),
                    QueryString = Maps("addQueryString1:value")
                },
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Body = Maps("appendbody1:value"),
                    Headers = Maps("appendHeader1:value"),
                    QueryString = Maps("appendQueryString1:value")
                },
                Remove = new RequestTransformerAdvancedTransformRemove
                {
                    Body = new HashSet<string> { "removebody1" },
                    Headers = new HashSet<string> { "removeHeader1" },
                    QueryString = new HashSet<string> { "removeQueryString1" }
                },
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Body = Maps("renamebody1:value"),
                    Headers = Maps("renameHeader1:value"),
                    QueryString = Maps("renameQueryString1:value")
                },
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Body = Maps("replacebody1:value"),
                    Headers = Maps("replaceHeader1:value"),
                    QueryString = Maps("replaceQueryString1:value"),
                    Uri = "bob.com"
                }
            };

            var plugin = new JeRequestTransformerPlugin();

            var configOut = PluginHelpers.RoundTripFromConfig(plugin, configIn);

            configOut.IsExactMatch(configIn).Should().BeTrue();
        }

        [Fact]
        public void RoundTripFromBodyWithNoData()
        {
            PluginHelpers.RoundTripFromBodyTest(new JeRequestTransformerPlugin());
        }

        private static IDictionary<string, string> Maps(params string[] values)
        {
            return ConfigReadExtensions.StringsToMaps(values);
        }
    }
}
