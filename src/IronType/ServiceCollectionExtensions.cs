namespace IronType;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIronType(this IServiceCollection serviceCollection, Action<IronTypeConfigurationBuilder>? configure)
    {
        var builder = new IronTypeConfigurationBuilder();

        configure?.Invoke(builder);

        var config = builder.Build();

        serviceCollection.AddSingleton(sp =>
        {
            return new TypeDataAdapter(config, Activate, Activate);

            object? Activate(Type type) => sp.GetService(type) ?? Activator.CreateInstance(type);
        });

        return serviceCollection;
    }
}
