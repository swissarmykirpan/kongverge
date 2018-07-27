namespace Kongverge.KongPlugin
{
    public interface IKongPluginConfig
    {
        bool IsExactMatch(IKongPluginConfig other);

#pragma warning disable IDE1006 // Disabled because json value is lowercase, but don't want to force a dependency on everyone
        string id { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
