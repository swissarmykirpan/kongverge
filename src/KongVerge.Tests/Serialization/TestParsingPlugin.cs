using System.Linq;
using Kongverge.KongPlugin;

namespace KongVerge.Tests.Serialization
{
    internal class TestParsingPlugin : KongPluginBase<TestKongConfig>
    {
        public TestParsingPlugin(string sectionName) : base(sectionName)
        {
        }

        public override string PluginName => throw new System.NotImplementedException();

        public override TestKongConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new TestKongConfig()
            {
                Value = pluginBody.config.Values.First().ToString()
            };
        }

        public override PluginBody DoCreatePluginBody(TestKongConfig target)
        {
            return new PluginBody(this.PluginName, new System.Collections.Generic.Dictionary<string, object>
            {
                {"Value", target.Value }
            });
        }
    }

    internal class OtherTestParsingPlugin : KongPluginBase<OtherTestKongConfig>
    {
        public OtherTestParsingPlugin(string sectionName) : base(sectionName)
        {
        }

        public override string PluginName => throw new System.NotImplementedException();

        public override OtherTestKongConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new OtherTestKongConfig()
            {
                Value = pluginBody.config.Values.OfType<int>().First()
            };
        }

        public override PluginBody DoCreatePluginBody(OtherTestKongConfig target)
        {
            return new PluginBody(this.PluginName, new System.Collections.Generic.Dictionary<string, object>
            {
                {"Value", target.Value }
            });
        }
    }

    internal class NestedTestParsingPlugin : KongPluginBase<NestedTestKongConfig>
    {
        public NestedTestParsingPlugin(string sectionName) : base(sectionName)
        {
        }

        public override string PluginName => throw new System.NotImplementedException();

        public override NestedTestKongConfig DoCreateConfigObject(PluginBody pluginBody)
        {
            return new NestedTestKongConfig()
            {
                Value = pluginBody.config.Values.OfType<int>().First(),
                Nested = pluginBody.config.Values.OfType<TestKongConfig>().First()
            };
        }

        public override PluginBody DoCreatePluginBody(NestedTestKongConfig target)
        {
            return new PluginBody(this.PluginName, new System.Collections.Generic.Dictionary<string, object>
            {
                {"Value", target.Value }
            });
        }
    }
}
