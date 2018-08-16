using System;

namespace Kongverge.KongPlugin
{
    public abstract class KongPluginBase<TConfig> : IKongPlugin
        where TConfig : IKongPluginConfig
    {
        protected KongPluginBase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Plugin name cannot be null or whitespace.");

            PluginName = name;
        }

        public string PluginName { get; }

        public Type KongObjectType => typeof(TConfig);

        public IKongPluginConfig CreateConfigObject(PluginBody pluginBody)
        {
            return DoCreateConfigObject(pluginBody);
        }
        
        // TODO: Can't we get rid of this method and add a type parameter to IKongPlugin instead?
        public PluginBody CreatePluginBody(IKongPluginConfig target)
        {
            var realTarget = (TConfig)target;
            var result = DoCreatePluginBody(realTarget);

            // Make sure nobody forgets to set this.
            result.name = PluginName;
            return result;
        }

        protected abstract TConfig DoCreateConfigObject(PluginBody pluginBody);

        protected abstract PluginBody DoCreatePluginBody(TConfig target);
    }
}
