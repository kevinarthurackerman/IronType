namespace IronType.NodaTime;

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfigurationBuilder UseNodaTime(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder)
    {
        ironTypeConfigurationBuilder.TypeData.Add(new TypeData<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)));
        ironTypeConfigurationBuilder.TypeData.Add(new TypeData<LocalDate, DateOnly>(x => x.ToDateOnly(), x => LocalDate.FromDateOnly(x)));

        return ironTypeConfigurationBuilder;
    }
}
