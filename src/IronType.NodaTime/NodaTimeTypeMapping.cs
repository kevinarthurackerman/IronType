namespace IronType.NodaTime;

internal class NodaTimeTypeMapping<TApp, TFramework> : TypeMapping<TApp, TFramework>
{
    public NodaTimeTypeMapping(
        Func<TApp, TFramework> convertToFrameworkValue,
        Func<TFramework, TApp> convertToAppValue)
        : base(convertToFrameworkValue, convertToAppValue)
    { }
}
