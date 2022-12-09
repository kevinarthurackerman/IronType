namespace IronType;

public class IronTypeConfigurationBuilder
{
    public IList<ITypeData> TypeData { get; } = new List<ITypeData>();

    public IronTypeConfiguration Build() => new(TypeData);
}
