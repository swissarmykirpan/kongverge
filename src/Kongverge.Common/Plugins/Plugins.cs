using System.Collections.Generic;
using Kongverge.KongPlugin;

namespace Kongverge.Common.Plugins
{
    public static class Plugins
    {
        public static PluginBody Http => new PluginBody("http-log", new Dictionary<string, object>{
            {"test", 1 }
        });
    }
}
