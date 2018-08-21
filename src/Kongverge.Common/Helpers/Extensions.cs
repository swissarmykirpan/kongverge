using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Serilog;

namespace Kongverge.Common.Helpers
{
    public class Variance
    {
        public string Field { get; set; }
        public object Existing { get; set; }
        public object New { get; set; }
    }

    public static class GenericExtensions
    {
        public static T FromJsonString<T>(this string value) where T : Enum
        {
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                if (string.Equals(value, enumValue.ToJsonString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return enumValue;
                }
            }

            var message = $"Invalid value for {typeof(T).Name} enum: '{value}'";
            Log.Error(message);
            throw new InvalidOperationException(message);
        }

        public static string ToJsonString<T>(this T value) where T : Enum
        {
            var attributes = (DescriptionAttribute[])typeof(T).GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0
                ? attributes[0].Description
                : value.ToString().ToLowerInvariant();
        }

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
    }
}
