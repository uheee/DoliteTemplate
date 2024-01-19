using System.Text.Json.Nodes;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DoliteTemplate.Api.Shared.Swagger;

/// <summary>
///     OpenAPI JsonNode样例生成器
/// </summary>
public class JsonNodeSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(JsonNode))
        {
            schema.Example = new OpenApiObject
            {
                {"str1", new OpenApiString("some string")},
                {"num1", new OpenApiInteger(123)},
                {"obj1", new OpenApiObject()},
                {"arr1", new OpenApiArray()}
            };
        }
    }
}