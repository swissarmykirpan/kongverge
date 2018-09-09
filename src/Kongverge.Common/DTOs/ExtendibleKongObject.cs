using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public class ExtendibleKongObject : KongObject
    {
        [JsonProperty("plugins")]
        public IReadOnlyList<KongPlugin> Plugins { get; set; } = Array.Empty<KongPlugin>();

        public override void StripPersistedValues()
        {
            base.StripPersistedValues();
            foreach (var plugin in Plugins)
            {
                plugin.StripPersistedValues();
            }
        }

        public virtual void AssignParentId(KongPlugin plugin)
        {
            plugin.ConsumerId = null;
            plugin.ServiceId = null;
            plugin.RouteId = null;
        }
    }
}
