using System.Linq;
using Kongverge.KongPlugin;

namespace KongVerge.Tests.Serialization
{
    internal class TestParsingPlugin : KongPluginBase<TestKongConfig>
    {
        public TestParsingPlugin(string name) : base(name)
        {
        }

        protected override TestKongConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new TestKongConfig
            {
                Value = pluginBody.config.Values.First().ToString()
            };
        }

        protected override PluginBody DoCreatePluginBody(TestKongConfig target)
        {
            return new PluginBody(PluginName, new System.Collections.Generic.Dictionary<string, object>
            {
                { "Value", target.Value }
            });
        }
    }

    internal class OtherTestParsingPlugin : KongPluginBase<OtherTestKongConfig>
    {
        public OtherTestParsingPlugin(string name) : base(name)
        {
        }

        protected override OtherTestKongConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new OtherTestKongConfig
            {
                Value = pluginBody.config.Values.OfType<int>().First()
            };
        }

        protected override PluginBody DoCreatePluginBody(OtherTestKongConfig target)
        {
            return new PluginBody(PluginName, new System.Collections.Generic.Dictionary<string, object>
            {
                { "Value", target.Value }
            });
        }
    }

    internal class NestedTestParsingPlugin : KongPluginBase<NestedTestKongConfig>
    {
        public NestedTestParsingPlugin(string name) : base(name)
        {
        }

        protected override NestedTestKongConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new NestedTestKongConfig
            {
                Value = pluginBody.config.Values.OfType<int>().First(),
                Nested = pluginBody.config.Values.OfType<TestKongConfig>().First()
            };
        }

        protected override PluginBody DoCreatePluginBody(NestedTestKongConfig target)
        {
            return new PluginBody(PluginName, new System.Collections.Generic.Dictionary<string, object>
            {
                { "Value", target.Value }
            });
        }
    }
}
