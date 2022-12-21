namespace IronType.EntityFramework.Relational;

public class IronTypeDbContextOptionsExtension : IDbContextOptionsExtension
{
    private readonly IronTypeConfiguration _ironTypeConfiguration;
    private readonly IEnumerable<Type> _frameworkTypes;

    public DbContextOptionsExtensionInfo Info => new IronTypeDbContextOptionsExtensionInfo(this);

    public IronTypeDbContextOptionsExtension(IronTypeConfiguration ironTypeConfiguration, IEnumerable<Type> frameworkTypes)
    {
        _ironTypeConfiguration = ironTypeConfiguration;
        _frameworkTypes = frameworkTypes.ToArray();
    }

    public void ApplyServices(IServiceCollection services)
    {
        services.AddSingleton<IRelationalTypeMappingSourcePlugin>(sp => new IronTypeRelationalTypeMappingSourcePlugin(sp, _ironTypeConfiguration, _frameworkTypes));
    }

    public void Validate(IDbContextOptions options)
    { 

    }

    private class IronTypeDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public IronTypeDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension) { }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment => throw new NotImplementedException();

        public override int GetServiceProviderHashCode() => 0;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            => debugInfo[nameof(IronTypeDbContextOptionsExtension)] = "1";

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => true;
    }
}
