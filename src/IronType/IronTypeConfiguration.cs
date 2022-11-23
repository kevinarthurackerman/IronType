namespace IronType;

public class IronTypeConfiguration
{
    public ImmutableList<Type> TypeDataTypes { get; }

    public Action<AdaptationFailureContext>? OnAdaptationFailure { get; }

    internal IronTypeConfiguration(IronTypeConfigurationBuilder ironTypeConfiguration)
    {
        var invalidTypeData = ironTypeConfiguration.TypeDataTypes
            .Where(x => !x.IsTypeData())
            .ToArray();

        if (invalidTypeData.Any())
        {
            var innerExceptions = invalidTypeData
                .Select(x => new Exception($"'{x}' does not extend '{typeof(TypeData<,>)}'."))
                .ToArray();

            throw new AggregateException($"One or more '{typeof(Type)}' provided in '{nameof(IronTypeConfigurationBuilder)}.{nameof(TypeDataTypes)}' do not extend '{typeof(TypeData<,>)}'.", innerExceptions);
        }

        TypeDataTypes = ironTypeConfiguration.TypeDataTypes.ToImmutableList();

        OnAdaptationFailure = ironTypeConfiguration.OnAdaptationFailure;
    }
}
