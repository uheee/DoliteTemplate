using System.Net;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DoliteTemplate.Api.Shared.Swagger;

/// <summary>
///     OpenAPI IP地址样例生成器
/// </summary>
public class IpAddressSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(IPAddress))
        {
            schema.Example = new OpenApiString("0.0.0.0");
        }
    }
}