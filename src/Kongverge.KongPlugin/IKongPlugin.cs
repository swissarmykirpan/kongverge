using System;

namespace Kongverge.KongPlugin
{
    public interface IKongPlugin
    {
        string PluginName { get; }

        Type KongObjectType { get; }

        PluginBody CreatePluginBody(IKongPluginConfig target);

        IKongPluginConfig CreateConfigObject(PluginBody pluginBody);
    }
}
