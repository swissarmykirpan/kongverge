using System;

namespace Kongverge.Extension
{
    public abstract class ExtensionBase<TConfig> : IExtension
        where TConfig : IKongPluginConfig
    {
        protected ExtensionBase(string section)
        {
            SectionName = section;
        }

        public string SectionName { get; }

        public Type KongObjectType => typeof(TConfig);

        public abstract string PluginName { get; }

        public IKongPluginConfig CreateConfigObject(PluginBody pluginBody)
        {
            return DoCreateConfigObject(pluginBody);
        }

        public PluginBody CreatePluginBody(IKongPluginConfig target)
        {
            var realTarget = (TConfig)target;
            var result = DoCreatePluginBody(realTarget);

            // Make sure nobody forgets to set this.
            result.name = PluginName;
            return result;
        }

        public abstract TConfig DoCreateConfigObject(PluginBody pluginBody);

        public abstract PluginBody DoCreatePluginBody(TConfig target);
    }
}
