
# IronType

The goal of this library is to help avoid [primitive data obsession](https://en.wikipedia.org/wiki/Primitive_data_type) by making it easy to map custom types onto primitive and simple types that are commonly supported by libraries.

**Basic Usage:**
First, add a reference to IronType and any extension libraries you want to consume to your project file.
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
        .WithUnitsNet()
        .WithNodaTime()
        .WithAssemblyTypeMappings(typeof(ServiceCollectionExtensions).Assembly);

    config = configure(config);

    IronTypeConfiguration.Global = config;

    return config;
});
```

Then, apply the type mappings to your services.
```csharp
services.AddDbContext<AppDbContext>(x => x.UseIronType());

services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.UseIronType());

services.AddSwaggerGen(x => x.UseIronType());

app.UseSwagger().UseSwaggerUI();
```

And now the registered types are available for your DTOs throughout the stack!
```csharp
public readonly record struct OrderId(Guid Value);

public class OrderIdTypeMapping : SimpleTypeMapping<OrderId, Guid> { }

public class Order
{
    public OrderId Id { get; set; }
    public LocalDate OrderedOn { get; set; }
    public string CustomerName { get; set; } = null!;
    public Length Length { get; set; }
    public Length Width { get; set; }
    public Length Height { get; set; }
    public Mass Weight { get; set; }
}
```

To create your own custom mappings just extend `TypeMapping<TApp, TFramework>` and register it in your call to `AddIronType(...)`.


**Nuget packages:**

- [Coming Soon](https://www.nuget.org/packages/.../)