namespace IronType;

public class IronTypeConfigurationBuilder
{
    public IList<ITypeData> TypeData { get; } = new List<ITypeData>();

    public IronTypeConfiguration Build() => new(TypeData);
}

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfigurationBuilder AddTypeData(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder, ITypeData typeData)
    {
        ironTypeConfigurationBuilder.TypeData.Add(typeData);

        return ironTypeConfigurationBuilder;
    }
}