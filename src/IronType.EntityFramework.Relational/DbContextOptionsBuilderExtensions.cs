namespace IronType.EntityFramework.Relational;

public static class DbContextOptionsBuilderExtensions
{
    public static TOptionsBuilder UseIronType<TOptionsBuilder>(this TOptionsBuilder optionsBuilder, Action<UseIronTypeConfiguration>? configure = null)
        where TOptionsBuilder : DbContextOptionsBuilder
    {
        var config = new UseIronTypeConfiguration();
        configure?.Invoke(config);

        var ironTypeConfiguration = config.IronTypeConfiguration;
        if (ironTypeConfiguration == null)
        {
            var coreOpts = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()!;

            ironTypeConfiguration = coreOpts.InternalServiceProvider?.GetService<IronTypeConfiguration>()
                ?? coreOpts.ApplicationServiceProvider?.GetService<IronTypeConfiguration>();
        }

        ironTypeConfiguration ??= IronTypeConfiguration.Global;

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(new IronTypeDbContextOptionsExtension(ironTypeConfiguration, config.FrameworkTypes));

        return optionsBuilder;
    }
}

public class UseIronTypeConfiguration
{
    public IronTypeConfiguration? IronTypeConfiguration { get; set; }

    public IList<Type> FrameworkTypes { get; } = new List<Type>
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
        };
}