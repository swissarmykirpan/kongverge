using System.Collections.Generic;
using System.Linq;

namespace Kongverge.Common.Helpers
{
    public class Variance
    {
        public string Field { get; set; }
        public object Existing { get; set; }
        public object New { get; set; }
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
    }
}
