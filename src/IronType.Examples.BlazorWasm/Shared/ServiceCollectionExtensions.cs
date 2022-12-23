namespace IronType.Examples.BlazorWasm.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddIronType(x =>
        {
            var config = x.WithUnitsNet()
                .WithNodaTime()
                .WithTypeMapping(SimpleTypeMappingFactory.Create<OrderId, Guid>());

            IronTypeConfiguration.Global = config;

            return config;
        });

        return serviceCollection;
    }
}
