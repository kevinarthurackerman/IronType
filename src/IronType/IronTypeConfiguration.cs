namespace IronType;

public class IronTypeConfiguration
{
    public static IronTypeConfiguration Global { get; set; }
        = new(Array.Empty<ITypeMapping>());

    public ImmutableList<ITypeMapping> TypeMapping { get; }

    internal IronTypeConfiguration(IEnumerable<ITypeMapping> typeMapping)
    {
        TypeMapping = typeMapping.ToImmutableList();
    }
}
