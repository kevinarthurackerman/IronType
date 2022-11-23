namespace IronType.EntityFramework.Relational;

public static class DbContextOptionsBuilderExtensions
{
    public static TOptionsBuilder UseIronType<TOptionsBuilder>(this TOptionsBuilder optionsBuilder, Action<UseIronTypeConfiguration>? configure = null)
        where TOptionsBuilder : DbContextOptionsBuilder
    {
        var config = new UseIronTypeConfiguration();
        configure?.Invoke(config);

        var typeDataAdapter = config.TypeDataAdapter;

        if (typeDataAdapter == null)
        {
            var coreOpts = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()!;

            if (coreOpts.ApplicationServiceProvider == null)
                throw new InvalidOperationException($"No {nameof(CoreOptionsExtension.ApplicationServiceProvider)} is set. Call {nameof(DbContextOptionsBuilder)}.{nameof(DbContextOptionsBuilder.UseApplicationServiceProvider)} to set the {nameof(ServiceProvider)}.");

            typeDataAdapter = coreOpts.InternalServiceProvider?.GetService<TypeDataAdapter>()
                ?? coreOpts.ApplicationServiceProvider?.GetService<TypeDataAdapter>();
        }

        if (typeDataAdapter == null)
            throw new InvalidOperationException($"No {nameof(TypeDataAdapter)} is was configured, registered to the {nameof(CoreOptionsExtension.InternalServiceProvider)}, or registered to the {nameof(CoreOptionsExtension.ApplicationServiceProvider)}");

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(new AppDbContextOptionsExtension(typeDataAdapter, config.OnAdaptationFailure));

        return optionsBuilder;
    }

    public class UseIronTypeConfiguration
    {
        public TypeDataAdapter? TypeDataAdapter { get; set; }
        public Action<AdaptationFailureContext>? OnAdaptationFailure { get; set; }
    }
}