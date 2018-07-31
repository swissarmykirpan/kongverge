using System;
using System.Linq;

namespace Kongverge.KongPlugin
{
    public abstract class KongPluginBase<TConfig> : IKongPlugin
        where TConfig : IKongPluginConfig
    {
        protected KongPluginBase(string section)
        {
            SectionName = section;
        }

        public string SectionName { get; }

        public Type KongObjectType => typeof(TConfig);

        public abstract string[] PluginNames { get; }

        public IKongPluginConfig CreateConfigObject(PluginBody pluginBody)
        {
            return DoCreateConfigObject(pluginBody);
        }

        public PluginBody CreatePluginBody(IKongPluginConfig target)
        {
            var realTarget = (TConfig)target;
            var result = DoCreatePluginBody(realTarget);

            // Make sure nobody forgets to set this.
            result.name = result.name ?? PluginNames.First();
            return result;
        }

        protected abstract TConfig DoCreateConfigObject(PluginBody pluginBody);

        protected abstract PluginBody DoCreatePluginBody(TConfig target);
    }
}
