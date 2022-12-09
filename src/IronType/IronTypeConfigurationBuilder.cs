namespace IronType;

public class IronTypeConfigurationBuilder
{
    public bool SetGlobal { get; set; } = false;

    public IList<ITypeData> TypeData { get; } = new List<ITypeData>();

    public IronTypeConfiguration Build()
    {
        var config = new IronTypeConfiguration(TypeData);

        if (SetGlobal) IronTypeConfiguration.Global = config;

        return config;
    }
}

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfigurationBuilder SetGlobal(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder, bool setGlobal = true)
    {
        ironTypeConfigurationBuilder.SetGlobal = setGlobal;

        return ironTypeConfigurationBuilder;
    }

    public static IronTypeConfigurationBuilder AddTypeData(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder, ITypeData typeData)
    {
        ironTypeConfigurationBuilder.TypeData.Add(typeData);

        return ironTypeConfigurationBuilder;
    }
}