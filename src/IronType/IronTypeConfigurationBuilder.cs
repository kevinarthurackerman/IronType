namespace IronType;

public class IronTypeConfigurationBuilder
{
    public IList<Type> TypeDataTypes { get; } = new List<Type>();

    public Action<AdaptationFailureContext>? OnAdaptationFailure { get; set; }
        = context => throw new InvalidOperationException($"Unable to adapt application Type '{context.AppType}' to any framework Type for framework '{context.Framework}'.");

    public IronTypeConfiguration Build() => new(this);
}
