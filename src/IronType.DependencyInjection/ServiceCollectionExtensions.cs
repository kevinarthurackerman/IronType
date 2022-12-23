namespace IronType.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIronType(this IServiceCollection serviceCollection, Func<IronTypeConfiguration, IronTypeConfiguration>? configure = null)
    {
        var config = new IronTypeConfiguration();
        if (configure != null)
            config = configure.Invoke(config);

        serviceCollection.AddSingleton(config);

        return serviceCollection;
    }

    public static IServiceCollection AddIronType(this IServiceCollection serviceCollection, IronTypeConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);

        return serviceCollection;
    }
}