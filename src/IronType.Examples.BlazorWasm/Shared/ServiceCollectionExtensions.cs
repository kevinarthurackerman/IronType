namespace IronType.Examples.BlazorWasm.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddIronType(x =>
        {
            var config = x
                .WithUnitsNet()
                .WithNodaTime()
                .WithAssemblyTypeMappings(typeof(ServiceCollectionExtensions).Assembly);

            IronTypeConfiguration.Global = config;

            return config;
        });

        return serviceCollection;
    }
}
