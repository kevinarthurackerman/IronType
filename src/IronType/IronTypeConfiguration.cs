namespace IronType;

public class IronTypeConfiguration
{
    public static IronTypeConfiguration Global { get; set; } = new(Array.Empty<ITypeData>());

    public ImmutableList<ITypeData> TypeData { get; }

    internal IronTypeConfiguration(IEnumerable<ITypeData> typeData)
    {
        TypeData = typeData.ToImmutableList();
    }
}
