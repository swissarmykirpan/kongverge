using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;
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
                    Body = new HashSet<string> { "addbody1:value" },
                    Headers = new HashSet<string> { "addheader1:value" },
                    QueryString = new HashSet<string> { "addQueryString1:value" }
                },
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Body = new HashSet<string> { "appendbody1:value" },
                    Headers = new HashSet<string> { "appendHeader1:value" },
                    QueryString = new HashSet<string> { "appendQueryString1:value" }
                },
                Remove = new RequestTransformerAdvancedTransformRemove
                {
                    Body = new HashSet<string> { "removebody1" },
                    Headers = new HashSet<string> { "removeHeader1" },
                    QueryString = new HashSet<string> { "removeQueryString1" }
                },
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Body = new HashSet<string> { "renamebody1:value" },
                    Headers = new HashSet<string> { "renameHeader1:value" },
                    QueryString = new HashSet<string> { "renameQueryString1:value" }
                },
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Body = new HashSet<string> { "replacebody1:value" },
                    Headers = new HashSet<string> { "replaceHeader1:value" },
                    QueryString = new HashSet<string> { "replaceQueryString1:value" },
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
    }
}
