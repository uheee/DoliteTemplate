using System.Reflection;
using Microsoft.OpenApi.Models;

namespace DoliteTemplate.Api.Utils;

public static class OpenApiExtensions
{
    public static void ConfigureOpenApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            var info = configuration.GetSection("OpenApi").Get<OpenApiInfo>() ??
                       throw new Exception("Missing OpenApi info");
            options.SwaggerDoc(info.Version, info);
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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
        });
    }
}

public class OpenApiSecurityRequirementItem
{
    public OpenApiSecurityScheme Key { get; set; } = null!;
    public IList<string> Value { get; set; } = Array.Empty<string>();
}