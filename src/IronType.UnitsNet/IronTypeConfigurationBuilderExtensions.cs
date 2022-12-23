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

        var typeMappings = quantityTypes
            .Select(CreateTypeMapping)
            .ToArray();

        return ironTypeConfiguration.WithTypeMappings(typeMappings);

        static bool IsQuantityType(Type type)
            => !type.IsClass
                && type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQuantity<>));

        ITypeMapping CreateTypeMapping(Type quantityType)
            => (ITypeMapping)createTypeMappingMethod!
                .MakeGenericMethod(quantityType)
                .Invoke(null, Array.Empty<object>())!;
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

    private static TypeMapping<TQuantity, double> CreateTypeMapping<TQuantity>()
        where TQuantity : IQuantity
    {
        var baseUnit = (Enum?)typeof(TQuantity)
            .GetProperty("BaseUnit", BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null);

        if (baseUnit == null) throw new InvalidOperationException($"Base unit could not be located for quantity type '{typeof(TQuantity)}'.");

        var convertToFrameworkValue = CreateConvertToFrameworkValueFunc(baseUnit);
        var convertToAppValue = CreateConvertToAppValueFunc(baseUnit);

        return new UnitsNetTypeMapping<TQuantity, double>(convertToFrameworkValue, convertToAppValue);

        static Func<TQuantity, double> CreateConvertToFrameworkValueFunc(Enum baseUnit)
        {
            var paramToConvert = Expression.Parameter(typeof(TQuantity), "x");
            var baseUnitConst = Expression.Constant(baseUnit);

            var asMethod = typeof(TQuantity)
                .GetMethod(nameof(IQuantity.As), new[] { baseUnit.GetType() });

            if (asMethod == null) throw new InvalidOperationException($"'As' method could could not be located for quantity type '{typeof(TQuantity)}'.");

            var methodCall = Expression.Call(paramToConvert, asMethod, baseUnitConst);

            return Expression.Lambda<Func<TQuantity, double>>(methodCall, paramToConvert).Compile();
        }

        static Func<double, TQuantity> CreateConvertToAppValueFunc(Enum baseUnit)
        {
            var paramToConvert = Expression.Parameter(typeof(double), "x");
            var baseUnitConst = Expression.Constant(baseUnit);

            var fromMethod = typeof(TQuantity)
                .GetMethod("From", BindingFlags.Public | BindingFlags.Static, new[] { typeof(QuantityValue), baseUnit.GetType() });

            if (fromMethod == null) throw new InvalidOperationException($"'From' method could could not be located for quantity type '{typeof(TQuantity)}'.");

            var paramAsQuantityValue = Expression.Convert(paramToConvert, typeof(QuantityValue));

            var methodCall = Expression.Call(null, fromMethod, paramAsQuantityValue, baseUnitConst);

            return Expression.Lambda<Func<double, TQuantity>>(methodCall, paramToConvert).Compile();
        }
    }
}