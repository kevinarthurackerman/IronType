namespace IronType.NewtonsoftJson;

public static class JsonSerializerSettingsExtensions
{
    public static JsonSerializerSettings UseIronType(this JsonSerializerSettings jsonSerializerSettings, Func<UseIronTypeConfiguration, UseIronTypeConfiguration>? configure = null)
    {
        var config = new UseIronTypeConfiguration();
        if (configure != null)
            config = configure.Invoke(config);
        
        var ironTypeConfiguration = config.IronTypeConfiguration;

        ironTypeConfiguration ??= IronTypeConfiguration.Global;

        var createJsonConverterMethod = typeof(JsonSerializerSettingsExtensions)
                    .GetMethod(nameof(CreateJsonConverter), BindingFlags.NonPublic | BindingFlags.Static)!;

        var typeMappings = ironTypeConfiguration.GetTypeMappingsForFramework(config.FrameworkTypes);

        var converters = typeMappings
            .Select(InstantiateJsonConverter)
            .ToArray();
        
        foreach (var converter in converters)
            jsonSerializerSettings.Converters.Add(converter);

        return jsonSerializerSettings;

        JsonConverter InstantiateJsonConverter(ITypeMapping typeMapping)
            => (JsonConverter)createJsonConverterMethod
                .MakeGenericMethod(typeMapping.AppType, typeMapping.FrameworkType)
                .Invoke(null, new object?[] { typeMapping })!;
    }

    private static JsonConverter CreateJsonConverter<TApp, TFramework>(TypeMapping<TApp, TFramework> typeMapping)
        => new DelegatingJsonConverter<TApp, TFramework>(typeMapping.ConvertToFrameworkValue, typeMapping.ConvertToAppValue);
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