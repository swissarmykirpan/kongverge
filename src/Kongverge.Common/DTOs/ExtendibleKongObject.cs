using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public override bool IsMatch<T>(T other)
        {
            throw new NotImplementedException();
        }

        public virtual void AssignParentId(KongPlugin plugin)
        {
            plugin.ConsumerId = null;
            plugin.ServiceId = null;
            plugin.RouteId = null;
        }

        public virtual Task Validate(ICollection<string> errorMessages)
        {
            return Task.CompletedTask;
        }
    }
}
