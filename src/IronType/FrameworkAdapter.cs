namespace IronType;

public abstract class FrameworkAdapter
{
    public abstract string Name { get; }

    public abstract bool TryAdapt<TApp, TFramework>(AdaptationContext<TApp, TFramework> adaptationContext);

    public virtual Action<AdaptationFailureContext>? OnAdaptationFailure { get; }

    public readonly record struct AdaptationContext<TApp, TFramework>(TypeData<TApp, TFramework> TypeData, TypeActivator Activator);
}
