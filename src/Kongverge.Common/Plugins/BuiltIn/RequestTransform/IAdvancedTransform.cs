namespace Kongverge.Common.Plugins.BuiltIn.RequestTransform
{
    public interface IAdvancedTransform
    {
        bool IsExactMatch(IAdvancedTransform other);
    }
}
