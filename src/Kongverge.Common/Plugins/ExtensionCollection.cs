using System;
using System.Collections.Generic;
using System.Linq;
using Kongverge.Extension;
using Serilog;

namespace Kongverge.Common.Plugins
{
    public class ExtensionCollection : IExtensionCollection
    {
        private readonly Dictionary<string, IExtension> _nameLookup;
        private readonly Dictionary<Type, IExtension> _typeLookup;

        public ExtensionCollection(IEnumerable<IExtension> plugins)
        {
            _nameLookup = plugins.ToDictionary(p => p.PluginName, p => p);
            _typeLookup = plugins.ToDictionary(p => p.KongObjectType, p => p);
        }

        public PluginBody CreatePluginBody(IKongPluginConfig plugin)
        {
            var type = plugin.GetType();

            if (_typeLookup.TryGetValue(type, out var strategy))
            {
                return strategy.CreatePluginBody(plugin);
            }

            Log.Error("Cannot find Kongverge extension to handle plugin body of type {type}", type);
            return null;
        }

        public IKongPluginConfig TranslateToConfig(PluginBody arg)
        {
            if(_nameLookup.TryGetValue(arg.name, out var strategy))
            {
                var constructed = strategy.CreateConfigObject(arg);

                constructed.id = arg.id;

                return constructed;
            }

            Log.Error("Cannot find Kongverge extension to handle plugin type {name}", arg.name);
            return null;
        }
    }
}
