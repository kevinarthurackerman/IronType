namespace IronType;

public class TypeDataAdapter
{
    private readonly IronTypeConfiguration _ironTypeConfiguration;
    private readonly TypeActivator _typeDataActivator;
    private readonly TypeActivator _frameworkAdapterActivator;

    private static readonly MethodInfo _tryAdaptMethod = typeof(TypeDataAdapter)
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Single(x => x.Name == nameof(TryAdapt));

    public TypeDataAdapter(IronTypeConfiguration ironTypeConfiguration)
        : this(ironTypeConfiguration, Activator.CreateInstance, Activator.CreateInstance) { }

    public TypeDataAdapter(IronTypeConfiguration ironTypeConfiguration, TypeActivator typeDataActivator, TypeActivator frameworkAdapterActivator)
    {
        _ironTypeConfiguration = ironTypeConfiguration;
        _typeDataActivator = typeDataActivator;
        _frameworkAdapterActivator = frameworkAdapterActivator;
    }

    public void ExecuteAdapter(FrameworkAdapter frameworkAdapter)
    {
        var appTypeDataSets = _ironTypeConfiguration.TypeDataTypes
            .Select(x => _typeDataActivator(x))
            .Cast<TypeData>()
            .GroupBy(x => x.AppType)
            .Select(x => new TypeDataSet(x.Key, x.OrderBy(y => y.Priority).ToImmutableArray()))
            .OrderBy(x => x.AppType)
            .ToImmutableArray();

        foreach (var appTypeDataSet in appTypeDataSets)
        {
            var adapterFoundForAppType = false;

            foreach(var typeData in appTypeDataSet.TypeDatas)
            {
                var adaptationSuccessful = (bool)_tryAdaptMethod
                    .MakeGenericMethod(typeData.AppType, typeData.FrameworkType)
                    .Invoke(this, new object[] { frameworkAdapter, typeData })!;

                if (adaptationSuccessful)
                {
                    adapterFoundForAppType = true;
                    break;
                }
            }

            if (!adapterFoundForAppType)
            {
                var onFailure = frameworkAdapter.OnAdaptationFailure
                    ?? _ironTypeConfiguration.OnAdaptationFailure;

                onFailure?.Invoke(new(frameworkAdapter.Name, appTypeDataSet.AppType));
            }
        }
    }

    private bool TryAdapt<TApp, TFramework>(FrameworkAdapter typeDataAdapter, TypeData<TApp, TFramework> typeData)
    {
        var context = new FrameworkAdapter.AdaptationContext<TApp, TFramework>(typeData, _frameworkAdapterActivator);

        return typeDataAdapter.TryAdapt(context);
    }

    private readonly record struct TypeDataSet(Type AppType, ImmutableArray<TypeData> TypeDatas);
}
