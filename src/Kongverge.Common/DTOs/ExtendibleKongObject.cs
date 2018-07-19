using System.Collections.Generic;
using System.Linq;
using Kongverge.Extension;
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

        public IList<IKongPluginConfig> Extensions { get; set; }

        public override string ToString()
        {
            return Id;
        }

        private class EmptyExtendibleKongObject : ExtendibleKongObject
        {
            internal EmptyExtendibleKongObject()
            {
                Extensions = new List<IKongPluginConfig>();
            }

            protected override PluginBody DoDecoratePluginBody(PluginBody body)
            {
                return body;
            }
        }

        public static ExtendibleKongObject Empty { get; } = new EmptyExtendibleKongObject();

        public bool ShouldSerializeExtensions()
        {
            return false;
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
