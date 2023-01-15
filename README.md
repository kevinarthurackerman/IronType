
# IronType

**A simple library for enabling using custom and imported types throughout the stack.**

The goal of this library is to help avoid [primitive data obsession](https://en.wikipedia.org/wiki/Primitive_data_type) by making it easy to map custom types onto primitive and simple types that are commonly supported by libraries. Additionaly, save time and avoid lock-in by making it easy to import mappings or switch libraries and bring your type mappings over with you.

## Quick Start

First, reference IronType in your project file.
```csharp
<ProjectReference Include="..\IronType\IronType.csproj" />
```

Next, let's create a custom type and some type mappings.

```csharp
public readonly record struct OrderId(Guid Value);

public class Order
{
    public OrderId Id { get; set; }
}
```

Next, we well set up our configuration. By calling `.WithAssemblyTypeMappings(...)` we are telling it to automatically discover type mappings in the specified assembly (which we will create next).

**Without a service container**

```csharp
var config = new IronTypeConfiguration()
    .WithAssemblyTypeMappings(typeof(AssemblyMarkerType));
```

**With a service container**

```csharp
services.AddIronType(config => config
    .WithAssemblyTypeMappings(typeof(AssemblyMarkerType)));
```

Next, because our custom type is simple (containing one constructor with one argument and one public property) we can just mark it with a `[SimpleTypeMappingAttribute]` and IronType will create a simple type mapping for us.

```csharp
[SimpleTypeMapping]
public readonly record struct OrderId(Guid Value);
```

Now we are ready to use our type mapping. Let's pull in the extensions for EntityFramework and System.Text.Json by referencing them in our project file.

```csharp
<ProjectReference Include="..\IronType.EntityFramework\IronType.EntityFramework.csproj" />
<ProjectReference Include="..\IronType.SystemTextJson\IronType.SystemTextJson.csproj" />
```

And now in our configuration for each library let's tell it to use our type mappings.

**Without a service container**

```csharp
var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseIronType()
    .Options;

var dbContext = new AppDbContext(dbContextOptions);

var jsonOptions = new JsonSerializerOptions().UseIronType();
```

**With a service container (and API controllers in this example)**

```csharp
services.AddDbContext<AppDbContext>(x => x.UseIronType());

services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.UseIronType());
```

Ok, let's try it out. Add the `Order` entity to your `DbContext`, insert and retreive an instance, and serialize and deserialize it.

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}
```

**Without a service container**

```csharp
var order = new Order { Id = new OrderId(Guid.NewGuid()) };

dbContext.Add(order);

dbContext.SaveChanges();

var persistedOrder = dbContext.Orders.Single(x => x.Id == order.Id);

var serializedOrder = JsonSerializer.Serialize(persistedOrder, typeof(Order), jsonOptions);

var deserializedOrder = JsonSerializer.Deserialize<Order>(serializedOrder, jsonOptions);
```

Now let's return our `Order` entities from a controller.

**With a service container (in an API controller)**

```csharp
[ApiController]
[Route("[controller]")]
public class TestController
{
    private readonly AppDbContext _dbContext;

    public Handler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IEnumerable<Order> Get()
        => _dbContext.Orders.ToArray();
}
```

Congrats! Now you can keep adding type mappings and framework extensions and your types will just work throughout the stack.

## Creating a custom type mapping

For more complex types where the mapping can't be easily implied you will need to create a custom type mapping.

To do so, just create a custom type and mapper and let the framework do the rest.

```csharp
public record struct Location(decimal Latitude, decimal Longitude)
{
    public override string ToString() => $"({Latitude},{Longitude})";
}

public class Order
{
    public OrderId Id { get; set; }
    public Location Location { get; set; }
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
```

Following the previous example, insert and retreive an instance, and serialize and deserialize it.

## Pulling in type mappings for other libraries

First, add references to the relavent extension libraries to your project file.

```csharp
<ProjectReference Include="..\IronType.UnitsNet\IronType.UnitsNet.csproj" />
<ProjectReference Include="..\IronType.NodaTime\IronType.NodaTime.csproj" />
```

Then, add the types to your configuration from before.

```csharp
services.AddIronType(config => config
    .WithUnitsNet()
    .WithNodaTime()
    .WithAssemblyTypeMappings(typeof(AssemblyMarkerType)));
```

And to your entity type.

```csharp
public class Order
{
    public OrderId Id { get; set; }
    public Location Location { get; set; }
    public LocalDate OrderedOn { get; set; }
    public Length Length { get; set; }
    public Length Width { get; set; }
    public Length Height { get; set; }
    public Mass Weight { get; set; }
}
```

And again, the new types just work throughout the stack.

## Nuget Packages

- [Coming Soon](https://www.nuget.org/packages/.../)

## License
[MIT](/blob/master/LICENSE)

## What Do You Think?
I would love to hear feedback. Please reach out to me at [kevin@ackerman.ventures](mailto:kevin@ackerman.ventures)