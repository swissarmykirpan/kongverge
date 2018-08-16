using System;
using System.Collections.Generic;
using System.Linq;
using Kongverge.Common.DTOs;
using Kongverge.KongPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kongverge.Common.Helpers
{
    public class PluginConverter : JsonConverter
    {
        public PluginConverter(IEnumerable<IKongPlugin> parsers)
        {
            _parsers = parsers;
        }

        private readonly IEnumerable<IKongPlugin> _parsers;

        public override bool CanConvert(Type objectType)
        {
            return typeof(ExtendibleKongObject).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = JObject.Load(reader);
            if (!(jobj.ToObject(objectType) is ExtendibleKongObject extendibleObject))
            {
                return existingValue;
            }

            if (extendibleObject.Plugins == null)
            {
                extendibleObject.Plugins = new List<IKongPluginConfig>();
            }

            // All Aboard the Hack Train!!
            // If you don't pass the serializer nested routes don't get extensions. If you pass it at the top you end
            // up in an infinite loop
            if (objectType == typeof(KongService))
            {
                var routes = jobj.SelectToken("Routes");

                if (routes != null)
                {
                    var parsedRoutes = routes.ToObject<KongRoute[]>(serializer);

                    (extendibleObject as KongService).Routes = parsedRoutes;
                }
            }

            foreach (var parser in _parsers)
            {
                var token = jobj.SelectToken(parser.PluginName);

                if (token != null)
                {
                    extendibleObject.Plugins.Add(token.ToObject(parser.KongObjectType) as IKongPluginConfig);
                }
            }

            return extendibleObject;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.Formatting = Formatting.Indented;
            var token = JObject.FromObject(value);

            var extendible = value as ExtendibleKongObject;
            token.Remove(nameof(extendible.Plugins));

            foreach (var plugin in _parsers)
            {
                if (extendible.Plugins == null) continue;
                foreach (var extension in extendible.Plugins.Where(e => e.GetType() == plugin.KongObjectType))
                {
                    token.AddFirst(new JProperty(plugin.PluginName, JToken.FromObject(extension)));
                }
            }

            token.WriteTo(writer);
        }
    }
}
