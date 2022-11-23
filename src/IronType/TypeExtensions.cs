namespace IronType;

internal static class TypeExtensions
{
    internal static bool IsTypeData(this Type type)
    {
        if (type.IsAbstract) return false;

        var parentType = type;

        while (parentType != null)
        {
            if (parentType.IsGenericType && parentType.GetGenericTypeDefinition() == typeof(TypeData<,>))
                return true;

            parentType = parentType.BaseType;
        }

        return false;
    }
}
