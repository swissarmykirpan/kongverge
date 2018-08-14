using System.Collections.Generic;
using Kongverge.KongPlugin;
using Newtonsoft.Json.Linq;

namespace Kongverge.Common.Plugins.BuiltIn
{
    public abstract class BaseRequestTransformerPlugin<T> : KongPluginBase<T> where T : IRequestTransformerConfig, new()
    {
        protected BaseRequestTransformerPlugin(string section) : base(section)
        {
        }

        protected override T DoCreateConfigObject(PluginBody pluginBody)
        {
            var httpMethod = pluginBody.ReadConfigString("http_method");

            var removeData = pluginBody.config.SubProperties("remove");
            var renameData = pluginBody.config.SubProperties("rename");
            var addData = pluginBody.config.SubProperties("add");
            var appendData = pluginBody.config.SubProperties("append");

            var replaceData = pluginBody.config.SubProperties("replace");
            var replaceConfig = new RequestTransformerAdvancedTransformReplace
            {
                Uri = replaceData.ReadString("uri"),
                QueryString = replaceData.ReadStringSet("querystring"),
                Headers = replaceData.ReadStringSet("headers"),
                Body = replaceData.ReadStringSet("body")
            };

            return new T
            {
                HttpMethod = httpMethod,
                Remove = ReadSection(removeData),
                Replace = replaceConfig,
                Rename = ReadSection(renameData),
                Add = ReadSection(addData),
                Append = ReadSection(appendData)
            };
        }

        private static RequestTransformerAdvancedTransformBase ReadSection(IDictionary<string, object> section)
        {
            return new RequestTransformerAdvancedTransformBase
            {
                Headers = section.ReadStringSet("headers"),
                QueryString = section.ReadStringSet("querystring"),
                Body = section.ReadStringSet("body")
            };
        }

        private static JObject WriteSection(RequestTransformerAdvancedTransformBase section)
        {
            return new JObject
            {
                {"headers", string.Join(',', section.Headers)},
                {"querystring", string.Join(',', section.QueryString)},
                {"body", string.Join(',', section.Body)}
            };
        }

        protected override PluginBody DoCreatePluginBody(T target)
        {
            var replace = WriteSection(target.Replace);
            replace.Add("uri", target.Replace.Uri);

            return new PluginBody(PluginName, new Dictionary<string, object>
            {
                { "http_method", target.HttpMethod },

                { "remove", WriteSection(target.Remove) },
                { "replace", replace },
                { "rename", WriteSection(target.Rename) },
                { "add", WriteSection(target.Add) },
                { "append", WriteSection(target.Append) }
            });
        }
    }
}
