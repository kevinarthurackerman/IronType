namespace IronType.EntityFrameworkRelational;

public class IronTypeRelationalTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
{
    private readonly IImmutableDictionary<Type, RelationalTypeMapping> _relationalTypeMappingLookupByType;

    public IronTypeRelationalTypeMappingSourcePlugin(
        IServiceProvider serviceProvider,
        IronTypeConfiguration ironTypeConfiguration,
        IEnumerable<Type> frameworkTypes)
    {
        var typeMappings = ironTypeConfiguration.GetTypeMappingsForFramework(frameworkTypes);

        _relationalTypeMappingLookupByType = typeMappings
            .Select(InstantiateRelationalTypeMapping)
            .ToImmutableDictionary(x => x.ClrType);

        RelationalTypeMapping InstantiateRelationalTypeMapping(ITypeMapping typeMapping)
        {
            var initializeFunc = () => (RelationalTypeMapping)typeof(IronTypeRelationalTypeMappingSourcePlugin)
                .GetMethod(nameof(CreateTypeMapping), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(typeMapping.AppType, typeMapping.FrameworkType)
                .Invoke(null, new object?[] { typeMapping, serviceProvider })!;

            return new LazyInitializedRelationalTypeMapping(typeMapping.AppType, initializeFunc);
        }
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
