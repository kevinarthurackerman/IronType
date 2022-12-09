namespace IronType;

public sealed class TypeMapping<TApp, TFramework> : ITypeMapping
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

    public object? ConvertToFrameworkValue(object? appValue)
    {
        if (appValue is TApp typedAppValue)
            return _convertToFrameworkValue(typedAppValue);

        throw new ArgumentException($"'{nameof(appValue)}' must be of type '{typeof(TApp)}'.");
    }

    public object? ConvertToAppValue(object? frameworkValue)
    {
        if (frameworkValue is TFramework typedAppValue)
            return _convertToAppValue(typedAppValue);

        throw new ArgumentException($"'{nameof(frameworkValue)}' must be of type '{typeof(TFramework)}'.");
    }
}

public interface ITypeMapping
{
    public Type FrameworkType { get; }

    public Type AppType { get; }

    public object? ConvertToFrameworkValue(object? appValue);

    public object? ConvertToAppValue(object? frameworkValue);
}