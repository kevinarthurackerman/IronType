namespace IronType;

public class IronTypeConfiguration
{
    private static IronTypeConfiguration? _global;
    private static readonly object _setGlobalLock = new ();
    public static IronTypeConfiguration Global
    {
        get
        {
            if (_global == null)
                throw new InvalidOperationException($"{nameof(Global)}' '{typeof(IronTypeConfiguration)}' has not been initialized.");

            return _global;
        }
        set
        {
            if (_global != null)
                throw new InvalidOperationException($"{nameof(Global)}' '{typeof(IronTypeConfiguration)}' has already been initialized.");

            lock (_setGlobalLock)
            {
                if (_global != null)
                    throw new InvalidOperationException($"{nameof(Global)}' '{typeof(IronTypeConfiguration)}' has already been initialized.");

                _global = value;
            }
        }
    }

    public ImmutableList<ITypeMapping> TypeMappings { get; init; } = ImmutableList<ITypeMapping>.Empty;
}

public static class IronTypeConfigurationExtensions
{
    public static IronTypeConfiguration WithTypeMapping(this IronTypeConfiguration ironTypeConfiguration, ITypeMapping typeMapping)
        => new() { TypeMappings = ironTypeConfiguration.TypeMappings.Add(typeMapping) };

    public static IronTypeConfiguration WithTypeMappings(this IronTypeConfiguration ironTypeConfiguration, IEnumerable<ITypeMapping> typeMappings)
        => new() { TypeMappings = ironTypeConfiguration.TypeMappings.AddRange(typeMappings) };

    public static IronTypeConfiguration WithoutTypeMapping(this IronTypeConfiguration ironTypeConfiguration, ITypeMapping typeMapping)
        => new() { TypeMappings = ironTypeConfiguration.TypeMappings.Remove(typeMapping) };

    public static IronTypeConfiguration WithoutTypeMappings(this IronTypeConfiguration ironTypeConfiguration, IEnumerable<ITypeMapping> typeMappings)
        => new() { TypeMappings = ironTypeConfiguration.TypeMappings.RemoveRange(typeMappings) };

    public static IronTypeConfiguration WithAssemblyTypeMappings<TAssemblyMarker>(this IronTypeConfiguration ironTypeConfiguration, Func<Type, object?>? typeMappingActivator = null)
        => ironTypeConfiguration.WithAssemblyTypeMappings(typeof(TAssemblyMarker), typeMappingActivator);

    public static IronTypeConfiguration WithAssemblyTypeMappings(this IronTypeConfiguration ironTypeConfiguration, AssemblySource assemblySource, Func<Type,object?>? typeMappingActivator = null)
    {
        typeMappingActivator ??= (Type type) => Activator.CreateInstance(type);

        var assemblyTypemappings = assemblySource
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(ITypeMapping).IsAssignableFrom(x));

        var simpleMappedTypes = assemblySource
            .SelectMany(x => x.GetTypes())
            .Where(x => x.GetCustomAttribute<SimpleTypeMappingAttribute>() != null)
            .Select(x =>
            {
                var appType = x;

                var ctors = x.GetConstructors();

                if (ctors.Length == 1)
                {
                    var ctor = ctors[0];
                    var @params = ctor.GetParameters();

                    if (@params.Length == 1)
                    {
                        var frameworkType = @params[0].ParameterType;

                        return typeof(SimpleTypeMapping<,>).MakeGenericType(appType, frameworkType);
                    }
                }

                throw new InvalidOperationException($"'{appType}' is not valid as a simple mapped type. Simple mapped types must have a single constructor that takes a single parameter.");
            });

        var typeMappings = assemblyTypemappings
            .Concat(simpleMappedTypes)
            .Select(InstanciateTypeMapping)
            .ToArray();

        return new () { TypeMappings = ironTypeConfiguration.TypeMappings.AddRange(typeMappings) };

        ITypeMapping InstanciateTypeMapping(Type type)
        {
            var typeMapping = typeMappingActivator.Invoke(type);

            if (typeMapping == null)
                throw new InvalidOperationException($"Failed to activate Type '{type}'.");

            if (typeMapping is not ITypeMapping typedTypeMapping)
                throw new InvalidOperationException($"Type '{type}' does not implement '{typeof(ITypeMapping)}'.");

            return typedTypeMapping;
        }
    }
}