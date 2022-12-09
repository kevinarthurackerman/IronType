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

public static class SimpleTypeMappingFactory
{
    public static ITypeMapping Create<TApp, TFramework>()
    {
        var convertToAppValue = CreateConverter<TApp, TFramework>();
        var convertToFrameworkValue = CreateConverter<TFramework, TApp>();

        return new TypeMapping<TApp, TFramework>(convertToFrameworkValue, convertToAppValue);
    }

    public static Func<TFrom,TTo> CreateConverter<TTo,TFrom>()
    {
        var ctors = typeof(TTo).GetConstructors();
        
        if (ctors.Length == 1)
        {
            var ctor = ctors[0];
            var @params = ctor.GetParameters();

            if (@params.Length == 1 && @params[0].ParameterType == typeof(TFrom))
            {
                return (TFrom from) => (TTo)ctor.Invoke(new object[] { from! });
            }
        }

        var props = typeof(TFrom).GetProperties();

        if (props.Length == 1 && props[0].PropertyType == typeof(TTo))
        {
            var prop = props[0];

            return (TFrom from) => (TTo)prop.GetValue(from!)!;
        }

        throw new InvalidOperationException($"No supported conversion from '{typeof(TFrom)}' to '{typeof(TTo)}' found. Supported conversions are a single public constructor on '{typeof(TTo)}' that takes a single parameter '{typeof(TFrom)}', or a single property on '{typeof(TFrom)}' that returns a value of '{typeof(TTo)}'.");
    }
}