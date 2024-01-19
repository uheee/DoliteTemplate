using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DoliteTemplate.Api.Shared.Swagger;

/// <summary>
///     OpenAPI 路由前缀指示器
/// </summary>
public class PathPrefixInsertDocumentFilter(string prefix) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.Keys.ToList();
        foreach (var path in paths)
        {
            var pathToChange = swaggerDoc.Paths[path];
            swaggerDoc.Paths.Remove(path);
            swaggerDoc.Paths.Add($"{prefix}{path}", pathToChange);
        }
    }
}