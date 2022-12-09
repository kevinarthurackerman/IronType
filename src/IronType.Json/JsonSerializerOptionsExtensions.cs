namespace IronType.Json;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions UseIronType(this JsonSerializerOptions jsonSerializerOptions, Action<UseIronTypeConfiguration>? configure = null)
    {
        var config = new UseIronTypeConfiguration();
        configure?.Invoke(config);

        var frameworkTypesLookup = config.FrameworkTypes.ToImmutableHashSet();

        var ironTypeConfiguration = config.IronTypeConfiguration;

        ironTypeConfiguration ??= IronTypeConfiguration.Global;

        var converters = ironTypeConfiguration.TypeData
            .Where(x => frameworkTypesLookup.Contains(x.FrameworkType))
            .GroupBy(x => x.AppType)
            .Select(x => x.Last())
            .Select(x =>
            {
                return (JsonConverter)typeof(JsonSerializerOptionsExtensions)
                    .GetMethod(nameof(CreateJsonConverter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(x.AppType, x.FrameworkType)
                    .Invoke(null, new object?[] { x })!;
            })
            .ToArray();

        foreach (var converter in converters)
            jsonSerializerOptions.Converters.Add(converter);

        return jsonSerializerOptions;
    }

    private static JsonConverter CreateJsonConverter<TApp, TFramework>(TypeData<TApp, TFramework> typeData)
        => new DelegatingJsonConverter<TApp, TFramework>(typeData.ConvertToFrameworkValue, typeData.ConvertToAppValue);
}

public class UseIronTypeConfiguration
{
    public IronTypeConfiguration? IronTypeConfiguration { get; set; }

    public IList<Type> FrameworkTypes { get; } = new List<Type>
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
        };
}