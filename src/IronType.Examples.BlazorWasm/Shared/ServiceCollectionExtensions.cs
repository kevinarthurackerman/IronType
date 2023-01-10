namespace IronType.Examples.BlazorWasm.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIronTypeCore(this IServiceCollection serviceCollection, Func<IronTypeConfiguration, IronTypeConfiguration> configure)
    {
        serviceCollection.AddIronType(x =>
        {
            var config = x
                .WithUnitsNet()
                .WithNodaTime()
                .WithAssemblyTypeMappings(typeof(ServiceCollectionExtensions).Assembly);

            config = configure(config);

            IronTypeConfiguration.Global = config;

            return config;
        });

        return serviceCollection;
    }
}
