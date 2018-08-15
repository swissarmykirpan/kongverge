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

            await _fixture.ShouldRoundTripPlugInToKong(plugin);
        }
    }
}
