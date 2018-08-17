using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;
using Kongverge.Common.Plugins.BuiltIn.RequestTransform;

namespace Kongverge.Common.Tests.Plugins
{
    public static class BuilderExtensions
    {
        public static ISingleObjectBuilder<T> PopulateRequestTransformerConfig<T>(this ISingleObjectBuilder<T> builder)
            where T : BaseRequestTransformerConfig
        {
            return builder
                .With(x => x.Add, RandomBaseConfig<RequestTransformerAdvancedTransformBase>())
                .With(x => x.Append, RandomBaseConfig<RequestTransformerAdvancedTransformBase>())
                .With(x => x.Rename, RandomBaseConfig<RequestTransformerAdvancedTransformBase>())
                .With(x => x.Replace, RandomBaseConfig<RequestTransformerAdvancedTransformReplace>())
                .With(x => x.Remove, RandomRemoveConfig());
            
            TTransform RandomBaseConfig<TTransform>() where TTransform : RequestTransformerAdvancedTransformBase => Builder<TTransform>
                .CreateNew()
                .With(x => x.Body, RandomDictionary())
                .With(x => x.Headers, RandomDictionary())
                .With(x => x.QueryString, RandomDictionary())
                .Build();

            Dictionary<string, string> RandomDictionary() => Enumerable
                .Range(0, 3)
                .Select(x => GetRandom.String(10))
                .ToDictionary(x => x, x => x.GetHashCode().ToString());

            RequestTransformerAdvancedTransformRemove RandomRemoveConfig() => Builder<RequestTransformerAdvancedTransformRemove>
                .CreateNew()
                .With(x => x.Body, RandomHashSet())
                .With(x => x.Headers, RandomHashSet())
                .With(x => x.QueryString, RandomHashSet())
                .Build();

            HashSet<string> RandomHashSet() => new HashSet<string>(Enumerable.Range(0, 3).Select(x => GetRandom.String(10)));
        }
    }
}
