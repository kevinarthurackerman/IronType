namespace IronType.Examples.BlazorWasm.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddIronType(x => 
            x.SetGlobal()
            .AddNodaTime()
            .AddTypeData(SimpleTypeDataFactory.Create<OrderId, Guid>()));

        return serviceCollection;
    }
}
