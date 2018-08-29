using System;
using System.Collections.Generic;
using System.Linq;
using Kongverge.KongPlugin;
using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public abstract class ExtendibleKongObject
    {
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public long? Created { get; set; }

        public bool ShouldSerializeId()
        {
            // Don't deserialize it, do serialize it
            return string.IsNullOrEmpty(Id);
        }

        public IReadOnlyCollection<IKongPluginConfig> Plugins { get; set; } = Array.Empty<IKongPluginConfig>();

        public override string ToString()
        {
            return Id;
        }

        public PluginBody DecoratePluginBody(PluginBody body)
        {
            body = DoDecoratePluginBody(body);

            body.created_at = Created;

            return body;
        }

        protected abstract PluginBody DoDecoratePluginBody(PluginBody body);
    }

    public class ExtendibleKongObjectTargetPair
    {
        public ExtendibleKongObject Target { get; }

        public ExtendibleKongObject Existing { get; }

        public ExtendibleKongObjectTargetPair(ExtendibleKongObject target, IEnumerable<ExtendibleKongObject> existing)
        {
            Target = target;
            Existing = existing.FirstOrDefault(e => e.Equals(target));

            if (Existing != null)
            {
                PopulateMissingTargetFields();
            }
        }

        private void PopulateMissingTargetFields()
        {
            Target.Id = Existing.Id;
            Target.Created = Existing.Created;
        }
    }
}
