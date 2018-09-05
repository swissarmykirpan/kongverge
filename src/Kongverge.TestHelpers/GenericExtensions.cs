using Newtonsoft.Json;

namespace Kongverge.TestHelpers
{
    public static class GenericExtensions
    {
        public static T Clone<T>(this T instance) =>
            (T)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(instance), instance.GetType());
    }
}
