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
            new NodaTimeTypeMapping<ZonedDateTime?, string?>(x => x == null ? null : zonedDateTimePattern.Format(x.Value), x => x == null ? null : zonedDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<ZonedDateTime?, DateTimeOffset?>(x => x?.ToDateTimeOffset(), x => x == null ? null : ZonedDateTime.FromDateTimeOffset(x.Value)),
            new NodaTimeTypeMapping<OffsetDateTime, string>(x => offsetDateTimePattern.Format(x), x => offsetDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<OffsetDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => OffsetDateTime.FromDateTimeOffset(x)),
            new NodaTimeTypeMapping<OffsetDateTime?, string?>(x => x == null ? null : offsetDateTimePattern.Format(x.Value), x => x == null ? null : offsetDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<OffsetDateTime?, DateTimeOffset?>(x => x?.ToDateTimeOffset(), x => x == null ? null : OffsetDateTime.FromDateTimeOffset(x.Value)),
            new NodaTimeTypeMapping<LocalDateTime, string>(x => localDateTimePattern.Format(x), x => localDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalDateTime, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDateTime.FromDateTime(x)),
            new NodaTimeTypeMapping<LocalDateTime?, string?>(x => x == null ? null : localDateTimePattern.Format(x.Value), x => x == null ? null : localDateTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalDateTime?, DateTime?>(x => x?.ToDateTimeUnspecified(), x => x == null ? null : LocalDateTime.FromDateTime(x.Value)),
            new NodaTimeTypeMapping<LocalDate, string>(x => localDatePattern.Format(x), x => localDatePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)),
            new NodaTimeTypeMapping<LocalDate, DateOnly>(x => x.ToDateOnly(), x => LocalDate.FromDateOnly(x)),
            new NodaTimeTypeMapping<LocalDate?, string?>(x => x == null ? null : localDatePattern.Format(x.Value), x => x == null ? null : localDatePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalDate?, DateTime?>(x => x?.ToDateTimeUnspecified(), x => x == null ? null : LocalDate.FromDateTime(x.Value)),
            new NodaTimeTypeMapping<LocalDate?, DateOnly?>(x => x?.ToDateOnly(), x => x == null ? null : LocalDate.FromDateOnly(x.Value)),
            new NodaTimeTypeMapping<LocalTime, string>(x => localTimePattern.Format(x), x => localTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalTime, DateTime>(x => new DateTime(1900, 1, 1).AddTicks(x.TickOfDay), x => LocalTime.FromTicksSinceMidnight(x.TimeOfDay.Ticks)),
            new NodaTimeTypeMapping<LocalTime, TimeSpan>(x => x.ToTimeOnly().ToTimeSpan(), x => LocalTime.FromTimeOnly(TimeOnly.FromTimeSpan(x))),
            new NodaTimeTypeMapping<LocalTime, TimeOnly>(x => x.ToTimeOnly(), x => LocalTime.FromTimeOnly(x)),
            new NodaTimeTypeMapping<LocalTime?, string?>(x => x == null ? null : localTimePattern.Format(x.Value), x => x == null ? null : localTimePattern.Parse(x).Value),
            new NodaTimeTypeMapping<LocalTime?, DateTime?>(x => x == null ? null : new DateTime(1900, 1, 1).AddTicks(x.Value.TickOfDay), x => x == null ? null : LocalTime.FromTicksSinceMidnight(x.Value.TimeOfDay.Ticks)),
            new NodaTimeTypeMapping<LocalTime?, TimeSpan?>(x => x?.ToTimeOnly().ToTimeSpan(), x => x == null ? null : LocalTime.FromTimeOnly(TimeOnly.FromTimeSpan(x.Value))),
            new NodaTimeTypeMapping<LocalTime?, TimeOnly?>(x => x?.ToTimeOnly(), x => x == null ? null : LocalTime.FromTimeOnly(x.Value)),
            new NodaTimeTypeMapping<Period, string>(x => periodPattern.Format(x), x => periodPattern.Parse(x).Value),
            new NodaTimeTypeMapping<Period, DateTime>(x => new DateTime(1900, 1, 1).AddTicks(x.Ticks), x => Period.FromTicks(x.TimeOfDay.Ticks)),
            new NodaTimeTypeMapping<Period, TimeSpan>(x => TimeSpan.FromTicks(x.Ticks), x => Period.FromTicks(x.Ticks)),
            new NodaTimeTypeMapping<Period, TimeOnly>(x => new TimeOnly(x.Ticks), x => Period.FromTicks(x.Ticks)),
            new NodaTimeTypeMapping<Period?, string?>(x => x == null ? null : periodPattern.Format(x), x => x == null ? null : periodPattern.Parse(x).Value),
            new NodaTimeTypeMapping<Period?, DateTime?>(x => x == null ? null : new DateTime(1900, 1, 1).AddTicks(x.Ticks), x => x == null ? null : Period.FromTicks(x.Value.TimeOfDay.Ticks)),
            new NodaTimeTypeMapping<Period?, TimeSpan?>(x => x == null ? null : TimeSpan.FromTicks(x.Ticks), x => x == null ? null : Period.FromTicks(x.Value.Ticks)),
            new NodaTimeTypeMapping<Period?, TimeOnly?>(x => x == null ? null : new TimeOnly(x.Ticks), x => x == null ? null : Period.FromTicks(x.Value.Ticks)),
        };

        return ironTypeConfiguration.WithTypeMappings(typeMappings);
    }

    public static IronTypeConfiguration WithoutNodaTime(this IronTypeConfiguration ironTypeConfiguration)
    {
        var nodaTimeTypeMappings = ironTypeConfiguration.TypeMappings
            .Where(IsNodaTimeTypeMapping)
            .ToArray();

        return ironTypeConfiguration.WithoutTypeMappings(nodaTimeTypeMappings);

        static bool IsNodaTimeTypeMapping(ITypeMapping typeMapping)
        {
            var type = typeMapping.GetType();

            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NodaTimeTypeMapping<,>))
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}
