using System;
using Kongverge.Common.Helpers;

namespace Kongverge.Common.DTOs
{
    public interface IKongEquatable<T> : IEquatable<T>
    {
        object GetEqualityValues();
    }

    internal static class KongObjectExtensions
    {
        internal static bool KongEquals<T>(this IKongEquatable<T> instance, IKongEquatable<T> other) where T : KongObject
        {
            if (other == null)
                return false;

            if (ReferenceEquals(instance, other))
                return true;

            if (other.GetType() != instance.GetType())
                return false;

            var instanceSerialized = KongJsonConvert.SerializeObject(instance.GetEqualityValues());
            var otherSerialized = KongJsonConvert.SerializeObject(other.GetEqualityValues());

            return otherSerialized == instanceSerialized;
        }

        internal static bool KongEqualsObject<T>(this IKongEquatable<T> instance, object obj) where T : KongObject
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(instance, obj))
                return true;

            return obj is IKongEquatable<T> other && instance.KongEquals(other);
        }

        internal static int GetKongHashCode<T>(this IKongEquatable<T> instance) where T : KongObject =>
            KongJsonConvert.SerializeObject(instance.GetEqualityValues()).GetHashCode();
    }
}
