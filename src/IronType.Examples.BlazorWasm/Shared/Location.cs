namespace IronType.Examples.BlazorWasm.Shared;

public record struct Location(decimal Latitude, decimal Longitude)
{
    public override string ToString() => $"({Latitude},{Longitude})";
}

public class LocationTypeMapping : TypeMapping<Location, string>
{
    private static readonly Regex _locationRegex = new (@"^\((?<latitude>\d+(\.\d+){0,1}),(?<longitude>\d+(\.\d+){0,1})\)$");

    public LocationTypeMapping() : base(ConvertToFrameworkType, ConvertToAppType) { }

    private static string ConvertToFrameworkType(Location location) => location.ToString();

    private static Location ConvertToAppType(string locationString)
    {
        var parseResult = _locationRegex.Match(locationString);

        if (!parseResult.Success)
            throw new InvalidOperationException("Failed to parse the location. Format should be '(###.###,###.###)'");

        var latitude = decimal.Parse(parseResult.Groups["latitude"].Value);
        var longitude = decimal.Parse(parseResult.Groups["longitude"].Value);

        return new (latitude, longitude);
    }
}