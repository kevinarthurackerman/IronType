namespace IronType.Swagger;

internal class IronTypeDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemas = GetReferencedSchemas();

        var schemasToRemove = swaggerDoc.Components.Schemas.Keys
            .Where(x => !schemas.Contains(x))
            .ToArray();

        foreach (var schema in schemasToRemove)
            swaggerDoc.Components.Schemas.Remove(schema);

        ImmutableHashSet<string> GetReferencedSchemas()
        {
            var referencedSchemaIds = new HashSet<string>();

            foreach (var operation in swaggerDoc.Paths.SelectMany(x => x.Value.Operations).Select(x => x.Value))
            {
                foreach (var parameter in operation.Parameters)
                    AddSchema(parameter.Schema, context);

                foreach (var response in operation.Responses)
                    foreach (var content in response.Value.Content)
                        AddSchema(content.Value.Schema, context);
            }

            return referencedSchemaIds.ToImmutableHashSet();

            void AddSchema(OpenApiSchema schema, DocumentFilterContext context)
            {
                if (schema.Reference != null)
                {
                    if (referencedSchemaIds.Add(schema.Reference.Id))
                    {
                        var repoSchema = context.SchemaRepository.Schemas[schema.Reference.Id];

                        foreach (var property in repoSchema.Properties)
                            AddSchema(property.Value, context);
                    }
                }
                else if (schema.Items != null)
                {
                    AddSchema(schema.Items, context);
                }
            }
        }
    }
}