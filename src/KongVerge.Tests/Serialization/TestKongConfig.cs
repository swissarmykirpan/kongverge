using Kongverge.KongPlugin;

namespace KongVerge.Tests.Serialization
{
    internal class TestKongConfig : IKongPluginConfig
    {
        public string Value { get; set; }
        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            return ReferenceEquals(this, other);
        }
    }

    internal class OtherTestKongConfig : IKongPluginConfig
    {
        public int Value { get; set; }
        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            return ReferenceEquals(this, other);
        }
    }

    internal class NestedTestKongConfig : IKongPluginConfig
    {
        public int Value { get; set; }

        public TestKongConfig Nested { get; set; }

        public string id { get; set; }

        public bool IsExactMatch(IKongPluginConfig other)
        {
            return ReferenceEquals(this, other);
        }
    }
}
