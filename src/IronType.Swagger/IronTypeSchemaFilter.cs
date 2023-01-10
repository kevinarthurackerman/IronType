namespace IronType.Swagger;

internal class IronTypeSchemaFilter : ISchemaFilter
{
    private readonly ITypeMapping _typeMapping;

    public IronTypeSchemaFilter(ITypeMapping typeMapping)
    {
        _typeMapping = typeMapping;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != _typeMapping.AppType) return;

        var frameworkTypeSchema = context.SchemaGenerator.GenerateSchema(_typeMapping.FrameworkType, context.SchemaRepository);

        foreach (var prop in typeof(OpenApiSchema).GetProperties())
        {
            var get = prop.GetGetMethod();
            var set = prop.GetSetMethod();

            if (get == null || set == null) continue;

            var value = get.Invoke(frameworkTypeSchema, null);
            set.Invoke(schema, new[] { value });
        }
    }
}