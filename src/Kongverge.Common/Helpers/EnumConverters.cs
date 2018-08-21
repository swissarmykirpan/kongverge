using System;
using System.ComponentModel;
using Serilog;

namespace Kongverge.Common.Helpers
{
    public static class EnumConverters
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
    }
}
