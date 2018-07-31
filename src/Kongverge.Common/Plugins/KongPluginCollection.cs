using System;
using System.Collections.Generic;
using System.Linq;
using Kongverge.KongPlugin;
using Serilog;

namespace Kongverge.Common.Plugins
{
    public class KongPluginCollection : IKongPluginCollection
    {
        private readonly Dictionary<string, IKongPlugin> _nameLookup;
        private readonly Dictionary<Type, IKongPlugin> _typeLookup;

        public KongPluginCollection(IEnumerable<IKongPlugin> plugins)
        {
            _nameLookup = plugins.SelectMany(p => p.PluginNames.Select(name => new {name, plugin = p}))
                                 .ToDictionary(p => p.name, p => p.plugin);

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
