using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Kongverge.TestHelpers
{
    public static class Random
    {
        private static readonly System.Random Source = new System.Random(Environment.TickCount);

        public static string HttpMethod()
        {
            var methods = typeof(HttpMethod)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(x => x.GetValue(null).ToString())
                .ToArray();

            return methods[Source.Next(0, methods.Length - 1)];
        }
    }
}
