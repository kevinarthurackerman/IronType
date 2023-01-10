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

    public static IServiceCollection ConfigureIronType(this IServiceCollection serviceCollection, Func<IronTypeConfiguration, IronTypeConfiguration> configure)
    {
        var configServiceDescriptor = serviceCollection
            .FirstOrDefault(x => x.ServiceType == typeof(IronTypeConfiguration));

        if (configServiceDescriptor == null)
            throw new InvalidOperationException($"'{nameof(ServiceCollectionExtensions)}.{nameof(AddIronType)}' must be called before it can be configured.");

        if (configServiceDescriptor.ImplementationInstance is not IronTypeConfiguration config)
            throw new InvalidOperationException($"Service of Type '{nameof(IronTypeConfiguration)} was found, but the implementation was of the wrong Type.");

        config = configure.Invoke(config);

        var newConfigServiceDescriptor = new ServiceDescriptor(typeof(IronTypeConfiguration), config);

        serviceCollection.Replace(newConfigServiceDescriptor);

        return serviceCollection;
    }
}