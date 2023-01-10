using Microsoft.Extensions.DependencyInjection;

namespace IronType.Swagger;

public static class SwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions UseIronType(this SwaggerGenOptions swaggerGenOptions, Func<UseIronTypeConfiguration, UseIronTypeConfiguration>? configure = null)
    {
        var config = new UseIronTypeConfiguration();
        if (configure != null)
            config = configure.Invoke(config);

        var frameworkTypesLookup = config.FrameworkTypes.ToImmutableHashSet();

        var ironTypeConfiguration = config.IronTypeConfiguration;

        ironTypeConfiguration ??= IronTypeConfiguration.Global;

        var typeMappings = ironTypeConfiguration.TypeMappings
            .Where(IsFrameworkTypeMapping)
            .GroupBy(x => x.AppType)
            .Select(x => x.Last())
            .Select(x => x)
            .ToArray();

        foreach (var typeMapping in typeMappings)
        {
            var descriptor = new FilterDescriptor { Type = typeof(IronTypeSchemaFilter), Arguments = new object[] { typeMapping } };
            swaggerGenOptions.SchemaFilterDescriptors.Add(descriptor);
        }

        swaggerGenOptions.DocumentFilter<IronTypeDocumentFilter>();

        return swaggerGenOptions;

        bool IsFrameworkTypeMapping(ITypeMapping typeMapping)
            => frameworkTypesLookup.Contains(typeMapping.FrameworkType);
    }
}

public class UseIronTypeConfiguration
{
    private static readonly IImmutableList<Type> _defaultFrameworkTypes = new[]
        {
            typeof(bool),
            typeof(bool?),
            typeof(byte),
            typeof(byte?),
            typeof(byte[]),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(decimal),
            typeof(decimal?),
            typeof(double),
            typeof(double?),
            typeof(float),
            typeof(float?),
            typeof(Guid),
            typeof(Guid?),
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(sbyte),
            typeof(sbyte?),
            typeof(short),
            typeof(short?),
            typeof(string),
            typeof(uint),
            typeof(uint?),
            typeof(ulong),
            typeof(ulong?),
            typeof(ushort),
            typeof(ushort?)
        }.ToImmutableList();

    public IronTypeConfiguration? IronTypeConfiguration { get; init; }

    public IImmutableList<Type> FrameworkTypes { get; init; } = _defaultFrameworkTypes;
}