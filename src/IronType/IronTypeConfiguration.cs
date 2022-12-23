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
}