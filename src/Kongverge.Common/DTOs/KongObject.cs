using Newtonsoft.Json;

namespace Kongverge.Common.DTOs
{
    public abstract class KongObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("created_at")]
        public long? CreatedAt { get; set; }

        public virtual void StripPersistedValues()
        {
            Id = null;
            CreatedAt = null;
        }
    }
}
