namespace IronType;

public class IronTypeConfiguration
{
    public static IronTypeConfiguration Global { get; set; }
        = new(Array.Empty<ITypeMapping>(), Array.Empty<IMethodMapping>());

    public ImmutableList<ITypeMapping> TypeMapping { get; }

    public ImmutableList<IMethodMapping> MethodMappings { get; }

    internal IronTypeConfiguration(IEnumerable<ITypeMapping> typeMapping, IEnumerable<IMethodMapping> methodMappings)
    {
        TypeMapping = typeMapping.ToImmutableList();
        MethodMappings = methodMappings.ToImmutableList();
    }
}
