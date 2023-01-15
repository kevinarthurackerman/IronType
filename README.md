
# IronType

The goal of this library is to help avoid [primitive data obsession](https://en.wikipedia.org/wiki/Primitive_data_type) by making it easy to map custom types onto primitive and simple types that are commonly supported by libraries. Additionaly, this can also help avoid lock-in by making it easy to switch libraries and bring your type mappings over with you.

**Basic Usage:**
First, add a reference to IronType and any extension packages you want to consume to your project file.
```csharp
<ProjectReference Include="..\IronType\IronType.csproj" />
<ProjectReference Include="..\IronType.UnitsNet\IronType.UnitsNet.csproj" />
<ProjectReference Include="..\IronType.NodaTime\IronType.NodaTime.csproj" />
<ProjectReference Include="..\IronType.Json\IronType.Json.csproj" />
<ProjectReference Include="..\IronType.Swagger\IronType.Swagger.csproj" />
```

Next, configure the type mapping registrations in your service container.
```csharp
services.AddIronType(x =>
{
    var config = x
        .WithUnitsNet() // Adds mappings for types in the UnitsNet package.
        .WithNodaTime() // Adds mappings for types in the NodaTime package.
        .WithAssemblyTypeMappings(typeof(AssemblyMarkerType)); // Adds mappings from the assembly containing the marker type.

    IronTypeConfiguration.Global = config; // Sets the global configuration. This can be useful for applying mappings below when the extension does not have access to a service container and you don't want to explicitly pass the configuration.

    return config;
});
```

Then, apply the type mappings to your services.
```csharp
services.AddDbContext<AppDbContext>(x => x.UseIronType()); // Adds registered mappings to EntityFramework.

services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.UseIronType()); // Adds registered mappings to System.Text.Json.

services.AddSwaggerGen(x => x.UseIronType()); // Adds registered mappings to Swagger.

app.UseSwagger().UseSwaggerUI();
```

And now the registered types are available for your DTOs throughout the stack!
```csharp
public class Order
{
    public OrderId Id { get; set; } // App custom type, see below.
    public LocalDate OrderedOn { get; set; } // NodaTime type.
    public string CustomerName { get; set; } = null!; // Standard primitive type.
    public Length Length { get; set; } // UnitsNet type.
    public Length Width { get; set; } // UnitsNet type.
    public Length Height { get; set; } // UnitsNet type.
    public Mass Weight { get; set; } // UnitsNet type.
}

[SimpleTypeMapping] // Simple types can be mapped just by adding this attribute. A simple type is one which has a single constructor taking a single parameter, and has a single property. These are commonly used for ids and measures of a specific type (eg: PersonCount).
public readonly record struct OrderId(Guid Value); // App custom "simple" type.

// App custom "complex" type. Because this type is more complex we implement a custom mapper below.
public record struct Location(decimal Latitude, decimal Longitude)
{
    public override string ToString() => $"({Latitude},{Longitude})";
}

// Complex type mapping.
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
```

To create your own custom mappings just extend `TypeMapping<TApp, TFramework>` or mark simple types with the [SimpleTypeMappingAttribute] and register it in your call to `AddIronType(...)` either explicitly or with `WithAssemblyTypeMappings(...)`.


**Nuget packages:**

- [Coming Soon](https://www.nuget.org/packages/.../)