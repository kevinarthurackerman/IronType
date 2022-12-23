namespace IronType.EntityFramework.Relational;

public static class DbContextOptionsBuilderExtensions
{
    public static TOptionsBuilder UseIronType<TOptionsBuilder>(this TOptionsBuilder optionsBuilder, Func<UseIronTypeConfiguration, UseIronTypeConfiguration>? configure = null)
        where TOptionsBuilder : DbContextOptionsBuilder
    {
        var config = new UseIronTypeConfiguration();
        if (configure != null)
            config = configure.Invoke(config);

        var ironTypeConfiguration = config.IronTypeConfiguration;
        if (ironTypeConfiguration == null)
        {
            var coreOpts = optionsBuilder.Options.FindExtension<CoreOptionsExtension>();

            ironTypeConfiguration = coreOpts?.InternalServiceProvider?.GetService<IronTypeConfiguration>()
                ?? coreOpts?.ApplicationServiceProvider?.GetService<IronTypeConfiguration>();
        }

        ironTypeConfiguration ??= IronTypeConfiguration.Global;

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(new IronTypeDbContextOptionsExtension(ironTypeConfiguration, config.FrameworkTypes));

        return optionsBuilder;
    }
}

public class UseIronTypeConfiguration
{
    private static readonly IImmutableList<Type> _defaultFrameworkTypes = new []
        {
            typeof(bool),
            typeof(bool?),
            typeof(byte),
            typeof(byte?),
            typeof(byte[]),
            typeof(char),
            typeof(char?),
            typeof(char[]),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?),
            typeof(decimal),
            typeof(decimal?),
            typeof(double),
            typeof(double?),
            typeof(Guid),
            typeof(Guid?),
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(short),
            typeof(short?),
            typeof(string),
            typeof(TimeSpan),
            typeof(TimeSpan?)
        }.ToImmutableList();

    public IronTypeConfiguration? IronTypeConfiguration { get; init; }

    public IImmutableList<Type> FrameworkTypes { get; init; } = _defaultFrameworkTypes;
}