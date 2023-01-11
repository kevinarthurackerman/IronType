namespace IronType;

public class TypeMapping<TApp, TFramework> : ITypeMapping<TApp, TFramework>
{
    private readonly Func<TApp, TFramework> _convertToFrameworkValue;
    private readonly Func<TFramework, TApp> _convertToAppValue;

    public TypeMapping(Func<TApp, TFramework> convertToFrameworkValue, Func<TFramework, TApp> convertToAppValue)
    {
        _convertToFrameworkValue = convertToFrameworkValue;
        _convertToAppValue = convertToAppValue;
    }

    public Type FrameworkType => typeof(TFramework);

    public Type AppType => typeof(TApp);

    public TFramework ConvertToFrameworkValue(TApp appValue)
        => _convertToFrameworkValue(appValue);

    public TApp ConvertToAppValue(TFramework frameworkValue)
        => _convertToAppValue(frameworkValue);
}

public interface ITypeMapping<TApp, TFramework> : ITypeMapping
{
    public TFramework ConvertToFrameworkValue(TApp appValue);

    public TApp ConvertToAppValue(TFramework frameworkValue);
}

public interface ITypeMapping
{
    public Type FrameworkType { get; }

    public Type AppType { get; }
}