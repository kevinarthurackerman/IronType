namespace IronType.UnitsNet;

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfiguration WithUnitsNet(this IronTypeConfiguration ironTypeConfiguration)
    {
        var quantityTypes = typeof(IQuantity).Assembly.GetTypes()
            .Where(IsQuantityType)
            .ToArray();

        var createTypeMappingMethod = typeof(IronTypeConfigurationBuilderExtensions)
            .GetMethod(nameof(IronTypeConfigurationBuilderExtensions.CreateTypeMapping), BindingFlags.NonPublic | BindingFlags.Static)!;

        var createNullableTypeMappingMethod = typeof(IronTypeConfigurationBuilderExtensions)
            .GetMethod(nameof(IronTypeConfigurationBuilderExtensions.CreateNullableTypeMapping), BindingFlags.NonPublic | BindingFlags.Static)!;

        var typeMappings = quantityTypes
            .Select(CreateTypeMapping)
            .ToArray();

        var nullableTypeMappings = quantityTypes
            .Select(CreateNullableTypeMapping)
            .ToArray();

        return ironTypeConfiguration
            .WithTypeMappings(typeMappings)
            .WithTypeMappings(nullableTypeMappings);

        static bool IsQuantityType(Type type)
            => !type.IsClass
                && type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQuantity<>));

        ITypeMapping CreateTypeMapping(Type quantityType)
        {
            var unitType = quantityType.GetInterfaces()
                .Single(x => x.Name == $"IQuantity`1")
                .GetGenericArguments()[0]!;

            return (ITypeMapping)createTypeMappingMethod!
                .MakeGenericMethod(quantityType, unitType)
                .Invoke(null, Array.Empty<object>())!;
        }

        ITypeMapping CreateNullableTypeMapping(Type quantityType)
        {
            var unitType = quantityType.GetInterfaces()
                .Single(x => x.Name == $"IQuantity`1")
                .GetGenericArguments()[0]!;

            return (ITypeMapping)createNullableTypeMappingMethod!
                .MakeGenericMethod(quantityType, unitType)
                .Invoke(null, Array.Empty<object>())!;
        }
    }

    public static IronTypeConfiguration WithoutUnitsNet(this IronTypeConfiguration ironTypeConfiguration)
    {
        var unitsNetTypeMappings = ironTypeConfiguration.TypeMappings
            .Where(IsUnitsNetTypeMapping)
            .ToArray();

        return ironTypeConfiguration.WithoutTypeMappings(unitsNetTypeMappings);

        static bool IsUnitsNetTypeMapping(ITypeMapping typeMapping)
        {
            var type = typeMapping.GetType();

            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(UnitsNetTypeMapping<,>))
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }

    private static TypeMapping<TQuantity, double> CreateTypeMapping<TQuantity, TUnitType>()
        where TQuantity : IQuantity<TUnitType>
        where TUnitType : Enum
    {
        var baseUnit = (TUnitType)typeof(TQuantity)
            .GetProperty("BaseUnit", BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null)!;

        return new UnitsNetTypeMapping<TQuantity, double>(
            x => x.As(baseUnit), 
            x => (TQuantity)Quantity.From(x, baseUnit));
    }

    private static TypeMapping<TQuantity?, double?> CreateNullableTypeMapping<TQuantity, TUnitType>()
        where TQuantity : IQuantity<TUnitType>
        where TUnitType : Enum
    {
        var baseUnit = (TUnitType)typeof(TQuantity)
            .GetProperty("BaseUnit", BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null)!;

        return new UnitsNetTypeMapping<TQuantity?, double?>(
            x => x?.As(baseUnit),
            x => (TQuantity?)(x == null ? null : Quantity.From(x.Value, baseUnit)));
    }
}