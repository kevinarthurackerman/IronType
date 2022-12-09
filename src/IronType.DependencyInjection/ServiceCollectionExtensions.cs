namespace IronType.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIronType(this IServiceCollection serviceCollection, Action<IronTypeConfigurationBuilder>? configure = null)
    {
        var config = new IronTypeConfigurationBuilder();
        configure?.Invoke(config);

        serviceCollection.AddSingleton(config.Build());

        return serviceCollection;
    }

    public static IServiceCollection AddIronType(this IServiceCollection serviceCollection, IronTypeConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);

        return serviceCollection;
    }
}