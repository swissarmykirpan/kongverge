using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Kongverge.Extension;
using Newtonsoft.Json.Linq;

namespace Kongverge.Common.Helpers
{
    public class Variance
    {
        public string Field { get; set; }
        public object Existing { get; set; }
        public object New { get; set; }
    }

    public static class MyEnumExtensions
    {
        public static string ToDescriptionString(this PluginBody val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    public static class Extensions
    {
        public static List<Variance> DetailedCompare<T>(this T val1, T val2)
        {
            var fi = val1.GetType().GetProperties();

            return fi.Select(f => new Variance
                {
                    Field = f.Name,
                    Existing = f.GetValue(val1),
                    New = f.GetValue(val2)
                })
                .Where(v => v.Existing?.Equals(v.New) == false)
                .ToList();
        }

        public static int SequenceHash<T>(this IEnumerable<T> target)
        {
            return target?.Aggregate(0, (accumulator, item) => accumulator + item.GetHashCode()) ?? 0;
        }

        public static bool SafeEquivalent<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first == null)
            {
                return second?.Any() != true;
            }

            if (second == null)
            {
                return !first.Any();
            }

            return first.SequenceEqual(second);
        }

        public static IEnumerable<T> SafeIfNull<T>(this IEnumerable<T> sequence)
        {
            return sequence ?? Enumerable.Empty<T>();
        }

        public static T SafeCastJObjectProperty<T>(this JObject jObject, string property)
        {
            try
            {
                return jObject["headers"].ToObject<T>();
            }
            catch (Exception)
            {
            }

            return default;
        }
    }
}
