using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     OpenAPI扩展
/// </summary>
public static class OpenApiExtensions
{
    /// <summary>
    ///     配置OpenAPI
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置项</param>
    /// <exception cref="Exception">未设置OpenAPI配置项</exception>
    public static void ConfigureOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApiDocument(document =>
        {
            var openApiConfiguration = configuration.GetSection("OpenApi").Get<OpenApiConfiguration>();
            if (openApiConfiguration is null)
            {
                return;
            }

            document.Version = openApiConfiguration.Version;
            document.Title = openApiConfiguration.Title;
            document.Description = openApiConfiguration.Description;
            foreach (var (name, securityConfiguration) in openApiConfiguration.Security)
            {
                document.AddSecurity(name, securityConfiguration.Scopes, securityConfiguration.Scheme);
                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor(name));
            }
        });
    }
}

public class OpenApiConfiguration
{
    public string Version { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IDictionary<string, OpenApiSecurityConfiguration> Security { get; set; }
}

/// <summary>
///     OpenAPI安全需求配置
/// </summary>
public class OpenApiSecurityConfiguration
{
    public OpenApiSecurityScheme Scheme { get; set; }
    public string[] Scopes { get; set; } = [];
}