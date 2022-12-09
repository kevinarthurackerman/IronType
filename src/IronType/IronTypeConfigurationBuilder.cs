namespace IronType;

public class IronTypeConfigurationBuilder
{
    public bool SetGlobal { get; set; } = false;

    public IList<ITypeMapping> TypeMappings { get; } = new List<ITypeMapping>();

    public IronTypeConfiguration Build()
    {
        var config = new IronTypeConfiguration(TypeMappings);

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

    public static IronTypeConfigurationBuilder AddTypeMapping(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder, ITypeMapping typeMapping)
    {
        ironTypeConfigurationBuilder.TypeMappings.Add(typeMapping);

        return ironTypeConfigurationBuilder;
    }
}