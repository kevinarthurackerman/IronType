namespace IronType.EntityFramework.Relational;

public class AppRelationalTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Action<AdaptationFailureContext>? _onAdaptationFailure;
    private IImmutableDictionary<Type, RelationalTypeMapping>? _relationalTypeMappings;

    private State _state = State.Uninitialized;

    public AppRelationalTypeMappingSourcePlugin(IServiceProvider serviceProvider, Action<AdaptationFailureContext>? onAdaptationFailure)
    {
        _serviceProvider = serviceProvider;
        _onAdaptationFailure = onAdaptationFailure;
    }

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        if (_state == State.Initializing) return null;

        if (_state == State.Uninitialized) Initialize();

        if (mappingInfo.ClrType == null) return null;

        if (!_relationalTypeMappings!.TryGetValue(mappingInfo.ClrType, out var relationalTypeMapping))
            return null; 

        return relationalTypeMapping;
    }

    private void Initialize()
    {
        _state = State.Initializing;

        var typeMappingDeps = _serviceProvider.GetRequiredService<RelationalTypeMappingSourceDependencies>();
        var plugins = typeMappingDeps.Plugins
            .Where(x => x != this)
            .ToArray();

        var typeDataAdapter = _serviceProvider.GetRequiredService<TypeDataAdapter>();
        var adapter = new Adapter(_serviceProvider, _onAdaptationFailure);
        typeDataAdapter.ExecuteAdapter(adapter);
        _relationalTypeMappings = adapter.GetTypeMappings();

        _state = State.Initialized;
    }

    private enum State
    {
        Uninitialized,
        Initializing,
        Initialized
    }

    private class Adapter : FrameworkAdapter
    {
        private readonly Dictionary<Type, RelationalTypeMapping> _typeMappings = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly Action<AdaptationFailureContext>? _onAdaptationFailure;

        public override string Name => "EntityFramework";

        public override Action<AdaptationFailureContext>? OnAdaptationFailure => _onAdaptationFailure;

        internal Adapter(IServiceProvider serviceProvider, Action<AdaptationFailureContext>? onAdaptationFailure)
        {
            _serviceProvider = serviceProvider;
            _onAdaptationFailure = onAdaptationFailure;
        }

        public override bool TryAdapt<TApp, TFramework>(AdaptationContext<TApp, TFramework> adaptationContext)
        {
            var relationalTypeMappingSource = _serviceProvider
                .GetService<IRelationalTypeMappingSource>();
            
            if (relationalTypeMappingSource == null) return false;

            var typeMapping = (RelationalTypeMapping?)relationalTypeMappingSource
                .FindMapping(typeof(TFramework))
                ?.Clone(new RelationalTypeMappingInfo(typeof(TApp)))
                .Clone(new ValueConverter<TApp, TFramework>( 
                    x => adaptationContext.TypeData.ConvertToFrameworkValue(x),
                    x => adaptationContext.TypeData.ConvertToAppValue(x)));

            if (typeMapping == null) return false;

            _typeMappings.Add(typeof(TApp), typeMapping);

            return true;
        }

        internal IImmutableDictionary<Type, RelationalTypeMapping> GetTypeMappings()
            => _typeMappings.ToImmutableDictionary();
    }
}
