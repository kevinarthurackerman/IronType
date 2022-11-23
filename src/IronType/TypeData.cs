namespace IronType;

public abstract class TypeData<TApp, TFramework> : TypeData
{
    public abstract TFramework ConvertToFrameworkValue(TApp appValue);

    public abstract TApp ConvertToAppValue(TFramework frameworkValue);

    public sealed override Type FrameworkType => typeof(TFramework);

    public sealed override Type AppType => typeof(TApp);

    public sealed override object? ConvertToFrameworkValue(object? appValue)
        => ConvertToFrameworkValue(appValue);

    public sealed override object? ConvertToAppValue(object? appValue)
        => ConvertToAppValue(appValue);
}

public abstract class TypeData
{
    public virtual int Priority { get; } = 0;

    public abstract Type FrameworkType { get; }

    public abstract Type AppType { get; }

    public abstract object? ConvertToFrameworkValue(object? appValue);

    public abstract object? ConvertToAppValue(object? frameworkValue);

    public readonly record struct FrameworkAdapterFilterContext(string frameworkName);
}

public class SimpleTypeData<TApp, TFramework> : TypeData<TApp, TFramework>
{
    private readonly Func<TFramework, TApp> _convertToAppValue;
    private readonly Func<TApp, TFramework> _convertToFrameworkValue;

    public SimpleTypeData()
    {
        _convertToAppValue = CreateConverter<TApp, TFramework>();
        _convertToFrameworkValue = CreateConverter<TFramework, TApp>();
    }

    public Func<TFrom,TTo> CreateConverter<TTo,TFrom>()
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

    public override TApp ConvertToAppValue(TFramework frameworkValue)
        => _convertToAppValue(frameworkValue);

    public override TFramework ConvertToFrameworkValue(TApp appValue)
        => _convertToFrameworkValue(appValue);
}