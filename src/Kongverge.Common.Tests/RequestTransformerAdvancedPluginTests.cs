using System.Collections.Generic;
using FluentAssertions;
using Kongverge.Common.Plugins.BuiltIn;
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
                    Body = new HashSet<string> { "addbody1"},
                    Headers = new HashSet<string> { "addheader1"},
                    QueryString = new HashSet<string> { "addQueryString1"}
                },
                Append = new RequestTransformerAdvancedTransformBase
                {
                    Body = new HashSet<string> { "appendbody1" },
                    Headers = new HashSet<string> { "appendHeader1" },
                    QueryString = new HashSet<string> { "appendQueryString1" }
                },
                Remove = new RequestTransformerAdvancedTransformBase
                {
                    Body = new HashSet<string> { "removebody1" },
                    Headers = new HashSet<string> { "removeHeader1" },
                    QueryString = new HashSet<string> { "removeQueryString1" }
                },
                Rename = new RequestTransformerAdvancedTransformBase
                {
                    Body = new HashSet<string> { "renamebody1" },
                    Headers = new HashSet<string> { "renameHeader1" },
                    QueryString = new HashSet<string> { "renameQueryString1" }
                },
                Replace = new RequestTransformerAdvancedTransformReplace
                {
                    Body = new HashSet<string> { "replacebody1" },
                    Headers = new HashSet<string> { "replaceHeader1" },
                    QueryString = new HashSet<string> { "replaceQueryString1" },
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
            var plugin = new RequestTransformerAdvancedPlugin();
            var bodyIn = new PluginBody(plugin.PluginName, new Dictionary<string, object>());

            var bodyOut = PluginHelpers.RoundTripFromBody(plugin, bodyIn);

            bodyOut.Should().NotBeNull();
            bodyOut.name.Should().Be(bodyIn.name);
        }
    }
}
