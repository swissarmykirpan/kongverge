using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;

namespace Kongverge.TestHelpers
{
    public static class BuilderExtensions
    {
        public static ISingleObjectBuilder<T> PopulateRequestTransformerConfig<T>(this ISingleObjectBuilder<T> builder)
            where T : RequestTransformerConfig => builder
            .With(x => x.HttpMethod, RandomHttpMethod())
            .With(x => x.Add, RandomBaseConfig<AdvancedTransform>())
            .With(x => x.Append, RandomBaseConfig<AdvancedTransform>())
            .With(x => x.Rename, RandomBaseConfig<AdvancedTransform>())
            .With(x => x.Replace, RandomBaseConfig<AdvancedTransformReplace>())
            .With(x => x.Remove, RandomRemoveConfig());

        public static string RandomHttpMethod()
        {
            var methods = typeof(HttpMethod)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Select(x => x.GetValue(null).ToString())
                .ToArray();

            return methods[GetRandom.Int(0, methods.Length - 1)];
        }

        public static TTransform RandomBaseConfig<TTransform>() where TTransform : AdvancedTransform => Builder<TTransform>
            .CreateNew()
            .With(x => x.Body, RandomDictionary())
            .With(x => x.Headers, RandomDictionary())
            .With(x => x.QueryString, RandomDictionary())
            .Build();

        public static Dictionary<string, string> RandomDictionary(int length = 3) => Enumerable
            .Range(0, length)
            .Select(x => GetRandom.String(10))
            .ToDictionary(x => x, x => x.GetHashCode().ToString());

        public static AdvancedTransformRemove RandomRemoveConfig() => Builder<AdvancedTransformRemove>
            .CreateNew()
            .With(x => x.Body, RandomHashSet())
            .With(x => x.Headers, RandomHashSet())
            .With(x => x.QueryString, RandomHashSet())
            .Build();

        public static HashSet<string> RandomHashSet(int length = 3) => new HashSet<string>(Enumerable.Range(0, length).Select(x => GetRandom.String(10)));
    }
}
