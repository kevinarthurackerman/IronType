namespace IronType;

public class SimpleTypeMapping<TApp, TFramework> : TypeMapping<TApp, TFramework>
{
    public SimpleTypeMapping()
        : base(CreateConverter<TFramework, TApp>(), CreateConverter<TApp, TFramework>())
        { }

    private static Func<TFrom, TTo> CreateConverter<TTo, TFrom>()
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

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public class SimpleTypeMappingAttribute : Attribute { }