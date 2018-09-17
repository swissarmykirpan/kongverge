using System.Collections.Generic;
using System.Linq;
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

        public T MatchWithExisting<T>(IEnumerable<T> existingObjects) where T : KongObject
        {
            var existing = existingObjects.SingleOrDefault(x => x.IsMatch(this));
            if (existing != null)
            {
                Id = existing.Id;
                CreatedAt = existing.CreatedAt;
            }
            return existing;
        }

        public abstract bool IsMatch<T>(T other) where T : KongObject;
    }
}
