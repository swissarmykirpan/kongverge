using System.Collections.Generic;

namespace Kongverge.KongPlugin
{
    public class PluginBody
    {
        public string id;
        public long? created_at;

        public PluginBody(string name, Dictionary<string, object> config)
        {
            this.name = name;
            this.config = config;
        }

#pragma warning disable IDE1006 // Naming Styles
        public string consumer_id { get; set; }

        public string service_id { get; set; }

        public string route_id { get; set; }

        public string name { get; internal set; }

        public Dictionary<string, object> config { get; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
