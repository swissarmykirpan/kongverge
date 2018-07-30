using System;
using Newtonsoft.Json.Linq;

namespace Kongverge.KongPlugin
{
    public static class JsonExtensions
    {
        public static T SafeCastJObjectProperty<T>(this JObject jObject, string property)
        {
            try
            {
                return jObject[property].ToObject<T>();
            }
            catch (Exception)
            {
            }

            return default;
        }
    }
}
