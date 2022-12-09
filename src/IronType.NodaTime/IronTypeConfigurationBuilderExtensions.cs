namespace IronType.NodaTime;

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfigurationBuilder AddNodaTime(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder)
    {
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalDate, DateOnly>(x => x.ToDateOnly(), x => LocalDate.FromDateOnly(x)));

        return ironTypeConfigurationBuilder;
    }
}
