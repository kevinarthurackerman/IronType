namespace IronType.Examples.BlazorWasm.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddIronType(x => 
            x.SetGlobal()
            .AddNodaTime()
            .AddTypeMapping(SimpleTypeMappingFactory.Create<OrderId, Guid>()));

        return serviceCollection;
    }
}
