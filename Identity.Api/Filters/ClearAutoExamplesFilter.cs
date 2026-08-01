using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity.Api.Filters
{
    public class ClearAutoExamplesFilter:ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema is OpenApiSchema concreteSchema)
            {
                concreteSchema.Example = null;
            }
        }
    }
}
