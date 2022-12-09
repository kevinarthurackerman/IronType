namespace IronType.Json.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseIronTypeJson(this IServiceCollection serviceCollection, Action<UseIronTypeConfiguration>? configure = null)
    {
        var config = new UseIronTypeConfiguration();
        configure?.Invoke(config);

        serviceCollection.AddOptions();
        
        serviceCollection.AddTransient<IConfigureOptions<JsonOptions>>(sp =>
        {
            var ironTypeConfiguration = sp.GetService<IronTypeConfiguration>();

            return new IronTypeJsonOptionsConfigurator(x =>
            {
                x.UseIronType(y =>
                {
                    y.IronTypeConfiguration = config.IronTypeConfiguration ?? ironTypeConfiguration;
                    
                    foreach(var frameworkType in config.FrameworkTypes)
                        y.FrameworkTypes.Add(frameworkType);
                });
            });
        });

        return serviceCollection;
    }
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