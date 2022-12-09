namespace IronType.EntityFramework.Relational;

public class IronTypeRelationalTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
{
    private readonly IImmutableDictionary<Type, RelationalTypeMapping> _relationalTypeMappingLookupByType;

    public IronTypeRelationalTypeMappingSourcePlugin(
        IServiceProvider serviceProvider,
        IronTypeConfiguration ironTypeConfiguration,
        IEnumerable<Type> frameworkTypes)
    {
        var frameworkTypesLookup = frameworkTypes.ToImmutableHashSet();

        _relationalTypeMappingLookupByType = ironTypeConfiguration.TypeMapping
            .Where(x => frameworkTypesLookup.Contains(x.FrameworkType))
            .GroupBy(x => x.AppType)
            .Select(x => x.Last())
            .Select(x =>
            {
                var initializeFunc = () => (RelationalTypeMapping)typeof(IronTypeRelationalTypeMappingSourcePlugin)
                    .GetMethod(nameof(CreateTypeMapping), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(x.AppType, x.FrameworkType)
                    .Invoke(null, new object?[] { x, serviceProvider })!;

                return (RelationalTypeMapping)new LazyInitializedRelationalTypeMapping(x.AppType, initializeFunc);
            })
            .ToImmutableDictionary(x => x.ClrType);
    }

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        if (mappingInfo.ClrType == null) return null;

        _relationalTypeMappingLookupByType.TryGetValue(mappingInfo.ClrType, out var mapping);

        return mapping;
    }

    private static RelationalTypeMapping CreateTypeMapping<TApp, TFramework>(
        TypeMapping<TApp, TFramework> typeMapping,
        IServiceProvider serviceProvider)
    {
        var relationalTypeMappingSource = serviceProvider
            .GetRequiredService<IRelationalTypeMappingSource>();

        var frameworkTypeMapping = relationalTypeMappingSource
            .FindMapping(typeMapping.FrameworkType);

        if (frameworkTypeMapping == null)
            throw new InvalidOperationException($"Relational type mapping for framework type '{typeMapping.FrameworkType}' not found.");

        return (RelationalTypeMapping)frameworkTypeMapping
            .Clone(new RelationalTypeMappingInfo(typeMapping.AppType))
            .Clone(new ValueConverter<TApp, TFramework>(
                x => typeMapping.ConvertToFrameworkValue(x),
                x => typeMapping.ConvertToAppValue(x)));
    }
}
