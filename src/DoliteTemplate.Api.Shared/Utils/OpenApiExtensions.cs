using System.Reflection;
using DoliteTemplate.Api.Shared.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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
        services.AddSwaggerGen(options =>
        {
            var info = configuration.GetSection("OpenApi").Get<OpenApiInfo>() ??
                       throw new Exception("Missing OpenApi info");
            options.SwaggerDoc(info.Version, info);
            var xmlFilename = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "DoliteTemplate.Domain.xml"), true);
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "DoliteTemplate.Api.Shared.xml"), true);
            var securitySchema = configuration.GetSection("OpenApi:Security:Definition").Get<OpenApiSecurityScheme>();
            if (securitySchema is not null)
            {
                options.AddSecurityDefinition(securitySchema.Scheme, securitySchema);
            }

            var securityRequirementItems = configuration.GetSection("OpenApi:Security:Requirement")
                .Get<OpenApiSecurityRequirementItem[]>();
            if (securityRequirementItems is not null)
            {
                var securityRequirement = new OpenApiSecurityRequirement();
                foreach (var item in securityRequirementItems)
                {
                    securityRequirement.Add(item.Key, item.Value);
                }

                options.AddSecurityRequirement(securityRequirement);
            }

            options.SchemaFilter<IpAddressSchemaFilter>();
            options.SchemaFilter<JsonNodeSchemaFilter>();

            var requestRoutePrefix = configuration["OpenApi:RequestRoutePrefix"];
            if (!string.IsNullOrEmpty(requestRoutePrefix))
            {
                options.DocumentFilter<PathPrefixInsertDocumentFilter>(requestRoutePrefix);
            }
        });
    }
}

/// <summary>
///     OpenAPI安全需求配置
/// </summary>
public class OpenApiSecurityRequirementItem
{
    public OpenApiSecurityScheme Key { get; set; } = null!;
    public IList<string> Value { get; set; } = Array.Empty<string>();
}