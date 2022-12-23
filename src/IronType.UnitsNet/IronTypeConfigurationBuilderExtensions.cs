namespace IronType.UnitsNet;

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfigurationBuilder AddUnitsNet(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder)
    {
        var quantityTypes = typeof(IQuantity).Assembly.GetTypes()
            .Where(x => !x.IsClass
                && x.GetInterfaces().Any(y => y.IsGenericType
                    && y.GetGenericTypeDefinition() == typeof(IQuantity<>)))
            .ToArray();

        var createTypeMappingMethod = typeof(IronTypeConfigurationBuilderExtensions)
            .GetMethod(nameof(IronTypeConfigurationBuilderExtensions.CreateTypeMapping), BindingFlags.NonPublic | BindingFlags.Static)!;

        foreach (var quantityType in quantityTypes)
        {
            var typeMapping = (ITypeMapping)createTypeMappingMethod
                .MakeGenericMethod(quantityType)
                .Invoke(null, Array.Empty<object>())!;

            ironTypeConfigurationBuilder.AddTypeMapping(typeMapping);
        }

        return ironTypeConfigurationBuilder;
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

        return new TypeMapping<TQuantity, double>(convertToFrameworkValue, convertToAppValue);

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