namespace IronType;

public  class AssemblySource : IEnumerable<Assembly>
{
    private readonly ImmutableHashSet<Assembly> _assemblies;

    public AssemblySource(IEnumerable<Assembly> assemblies)
    {
        _assemblies = assemblies.ToHashSet().ToImmutableHashSet();
    }

    public IEnumerator<Assembly> GetEnumerator() => _assemblies.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _assemblies.GetEnumerator();

    public static implicit operator AssemblySource(Assembly assembly) => new (new[] { assembly });
    public static implicit operator AssemblySource(Assembly[] assemblies) => new(assemblies);
    public static implicit operator AssemblySource(Type type) => new(new[] { type.Assembly });
    public static implicit operator AssemblySource(Type[] types) => new(types.Select(x => x.Assembly));
}
