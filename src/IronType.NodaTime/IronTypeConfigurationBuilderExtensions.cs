namespace IronType.NodaTime;

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfiguration WithNodaTime(this IronTypeConfiguration ironTypeConfiguration)
    {
        var zonedDateTimePattern = ZonedDateTimePattern.GeneralFormatOnlyIso;
        var offsetDateTimePattern = OffsetDateTimePattern.GeneralIso;
        var localDateTimePattern = LocalDateTimePattern.GeneralIso;
        var localDatePattern = LocalDatePattern.Iso;
        var localTimePattern = LocalTimePattern.GeneralIso;
        var periodPattern = PeriodPattern.NormalizingIso;

        var typeMappings = new ITypeMapping[]
        {
            new NodaTimeTypeMapping<ZonedDateTime, string>(x => zonedDateTimePattern.Format(x), x => zonedDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<ZonedDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => ZonedDateTime.FromDateTimeOffset(x)),
            new NodaTimeTypeMapping<OffsetDateTime, string>(x => offsetDateTimePattern.Format(x), x => offsetDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<OffsetDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => OffsetDateTime.FromDateTimeOffset(x)),
            new NodaTimeTypeMapping<LocalDateTime, string>(x => localDateTimePattern.Format(x), x => localDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalDateTime, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDateTime.FromDateTime(x)),
            new NodaTimeTypeMapping<LocalDate, string>(x => localDatePattern.Format(x), x => localDatePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)),
            new NodaTimeTypeMapping<LocalDate, DateOnly>(x => x.ToDateOnly(), x => LocalDate.FromDateOnly(x)),
            new NodaTimeTypeMapping<LocalTime, string>(x => localTimePattern.Format(x), x => localTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalTime, DateTime>(x => new DateTime(1900, 1, 1).AddTicks(x.TickOfDay), x => LocalTime.FromTicksSinceMidnight(x.TimeOfDay.Ticks)),
            new NodaTimeTypeMapping<LocalTime, TimeSpan>(x => x.ToTimeOnly().ToTimeSpan(), x => LocalTime.FromTimeOnly(TimeOnly.FromTimeSpan(x))),
            new NodaTimeTypeMapping<LocalTime, TimeOnly>(x => x.ToTimeOnly(), x => LocalTime.FromTimeOnly(x)),
            new NodaTimeTypeMapping<Period, string>(x => periodPattern.Format(x), x => periodPattern.Parse(x).Value),
            new NodaTimeTypeMapping<Period, DateTime>(x => new DateTime(1900, 1, 1).AddTicks(x.Ticks), x => Period.FromTicks(x.TimeOfDay.Ticks)),
            new NodaTimeTypeMapping<Period, TimeSpan>(x => TimeSpan.FromTicks(x.Ticks), x => Period.FromTicks(x.Ticks)),
            new NodaTimeTypeMapping<Period, TimeOnly>(x => new TimeOnly(x.Ticks), x => Period.FromTicks(x.Ticks))
        };

        return ironTypeConfiguration.WithTypeMappings(typeMappings);
    }
}
