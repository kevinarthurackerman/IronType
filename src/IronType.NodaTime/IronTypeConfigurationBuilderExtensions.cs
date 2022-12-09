﻿namespace IronType.NodaTime;

public static class IronTypeConfigurationBuilderExtensions
{
    public static IronTypeConfigurationBuilder AddNodaTime(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder)
    {
        var zonedDateTimePattern = ZonedDateTimePattern.GeneralFormatOnlyIso;
        var offsetDateTimePattern = OffsetDateTimePattern.GeneralIso;
        var localDateTimePattern = LocalDateTimePattern.GeneralIso;
        var localDatePattern = LocalDatePattern.Iso;
        var localTimePattern = LocalTimePattern.GeneralIso;

        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<ZonedDateTime, string>(x => zonedDateTimePattern.Format(x), x => zonedDateTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<ZonedDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => ZonedDateTime.FromDateTimeOffset(x)));

        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<OffsetDateTime, string>(x => offsetDateTimePattern.Format(x), x => offsetDateTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<OffsetDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => OffsetDateTime.FromDateTimeOffset(x)));

        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalDateTime, string>(x => localDateTimePattern.Format(x), x => localDateTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalDateTime, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDateTime.FromDateTime(x)));

        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalDate, string>(x => localDatePattern.Format(x), x => localDatePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalDate, DateOnly>(x => x.ToDateOnly(), x => LocalDate.FromDateOnly(x)));

        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalTime, string>(x => localTimePattern.Format(x), x => localTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalTime, DateTime>(x => new DateTime(1900, 1, 1).Add(x.ToTimeOnly().ToTimeSpan()), x => LocalTime.FromTimeOnly(TimeOnly.FromDateTime(x))));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalTime, TimeSpan>(x => x.ToTimeOnly().ToTimeSpan(), x => LocalTime.FromTimeOnly(TimeOnly.FromTimeSpan(x))));
        ironTypeConfigurationBuilder.TypeMappings.Add(new TypeMapping<LocalTime, TimeOnly>(x => x.ToTimeOnly(), x => LocalTime.FromTimeOnly(x)));

        return ironTypeConfigurationBuilder;
    }
}
