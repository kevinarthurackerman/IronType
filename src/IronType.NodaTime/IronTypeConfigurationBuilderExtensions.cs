﻿namespace IronType.NodaTime;

public static class IronTypeConfigurationBuilderExtensions
{
    private static readonly MethodInfo _localDatePlusDaysMethodInfo = typeof(LocalDate).GetMethod(nameof(LocalDate.PlusDays))!;
    private static readonly MethodInfo _dateTimeAddDaysMethodInfo = typeof(DateTime).GetMethod(nameof(DateTime.AddDays))!;

    private static readonly MethodInfo _localDateAt = typeof(LocalDate).GetMethod(nameof(LocalDate.At))!;
    private static readonly MethodInfo _activatorCreateInstance
        = typeof(Activator).GetMethod(nameof(Activator.CreateInstance), new[] { typeof(Type), typeof(object?[]) })!;

    public static IronTypeConfigurationBuilder AddNodaTime(this IronTypeConfigurationBuilder ironTypeConfigurationBuilder)
    {
        var zonedDateTimePattern = ZonedDateTimePattern.GeneralFormatOnlyIso;
        var offsetDateTimePattern = OffsetDateTimePattern.GeneralIso;
        var localDateTimePattern = LocalDateTimePattern.GeneralIso;
        var localDatePattern = LocalDatePattern.Iso;
        var localTimePattern = LocalTimePattern.GeneralIso;

        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<ZonedDateTime, string>(x => zonedDateTimePattern.Format(x), x => zonedDateTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<ZonedDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => ZonedDateTime.FromDateTimeOffset(x)));

        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<OffsetDateTime, string>(x => offsetDateTimePattern.Format(x), x => offsetDateTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<OffsetDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => OffsetDateTime.FromDateTimeOffset(x)));

        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalDateTime, string>(x => localDateTimePattern.Format(x), x => localDateTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalDateTime, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDateTime.FromDateTime(x)));

        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalDate, string>(x => localDatePattern.Format(x), x => localDatePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalDate, DateOnly>(x => x.ToDateOnly(), x => LocalDate.FromDateOnly(x)));

        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalTime, string>(x => localTimePattern.Format(x), x => localTimePattern.Parse(x).Value));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalTime, DateTime>(x => new DateTime(1900, 1, 1).Add(x.ToTimeOnly().ToTimeSpan()), x => LocalTime.FromTimeOnly(TimeOnly.FromDateTime(x))));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalTime, TimeSpan>(x => x.ToTimeOnly().ToTimeSpan(), x => LocalTime.FromTimeOnly(TimeOnly.FromTimeSpan(x))));
        ironTypeConfigurationBuilder.AddTypeMapping(new TypeMapping<LocalTime, TimeOnly>(x => x.ToTimeOnly(), x => LocalTime.FromTimeOnly(x)));

        ironTypeConfigurationBuilder.AddMethodMapping(new MethodMapping(
            info => info.Method == _localDatePlusDaysMethodInfo,
            (info, ctx) => new MethodMappingInfo(
                ctx.Convert(info.Instance!, typeof(DateTime)),
                _dateTimeAddDaysMethodInfo,
                new[] { ctx.Convert(info.Arguments[0], typeof(double)) })));

        ironTypeConfigurationBuilder.AddMethodMapping(new MethodMapping(
            info => info.Method == _localDateAt,
            (info, ctx) => new MethodMappingInfo(
                null,
                _activatorCreateInstance,
                new[] { ctx.Constant(typeof(DateTime), typeof(Type)), ctx.Convert(info.Arguments[0], typeof(double)) })));

        return ironTypeConfigurationBuilder;
    }
}
