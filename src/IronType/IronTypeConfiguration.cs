namespace IronType;

public class IronTypeConfiguration
{
    public ImmutableList<ITypeData> TypeData { get; }

    internal IronTypeConfiguration(IEnumerable<ITypeData> typeData)
    {
        TypeData = typeData.ToImmutableList();
    }
}
