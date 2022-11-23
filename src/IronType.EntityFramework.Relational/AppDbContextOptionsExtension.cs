namespace IronType.EntityFramework.Relational;

public class AppDbContextOptionsExtension : IDbContextOptionsExtension
{
    private readonly TypeDataAdapter _typeDataAdapter;
    private readonly Action<AdaptationFailureContext>? _onAdaptationFailure;

    public DbContextOptionsExtensionInfo Info => new AppDbContextOptionsExtensionInfo(this);

    public AppDbContextOptionsExtension(TypeDataAdapter typeDataAdapter, Action<AdaptationFailureContext>? onAdaptationFailure)
    {
        _typeDataAdapter = typeDataAdapter;
        _onAdaptationFailure = onAdaptationFailure;
    }

    public void ApplyServices(IServiceCollection services)
    {
        services.AddSingleton(_typeDataAdapter);
        services.AddSingleton<IRelationalTypeMappingSourcePlugin>(sp => new AppRelationalTypeMappingSourcePlugin(sp, _onAdaptationFailure));
    }

    public void Validate(IDbContextOptions options) { }

    private class AppDbContextOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public AppDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension) { }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment => throw new NotImplementedException();

        public override int GetServiceProviderHashCode() => 0;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            => debugInfo[nameof(AppDbContextOptionsExtension)] = "1";

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => true;
    }
}
