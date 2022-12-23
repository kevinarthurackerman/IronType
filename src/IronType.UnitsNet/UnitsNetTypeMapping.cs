namespace IronType.UnitsNet;

internal class UnitsNetTypeMapping<TApp, TFramework> : TypeMapping<TApp, TFramework>
{
    public UnitsNetTypeMapping(
        Func<TApp, TFramework> convertToFrameworkValue,
        Func<TFramework, TApp> convertToAppValue)
        : base(convertToFrameworkValue, convertToAppValue)
    { }
}
