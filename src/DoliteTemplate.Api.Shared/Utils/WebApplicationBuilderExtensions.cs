using Asp.Versioning;
using CaseExtensions;
using DoliteTemplate.Domain.Shared.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;

namespace DoliteTemplate.Api.Shared.Utils;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureWebApi(this WebApplicationBuilder builder,
        Action<AspNetCoreOpenApiDocumentGeneratorSettings>? configureDocument = null)
    {
        var configuration = builder.Configuration;
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new RouteKebabCaseTransformer()));
            const string serviceRouteToken = "service";
            options.Conventions.Add(new CustomRouteToken(serviceRouteToken, c =>
                (c.ControllerName.EndsWith(serviceRouteToken, StringComparison.OrdinalIgnoreCase)
                    ? c.ControllerName[..^serviceRouteToken.Length]
                    : c.ControllerName).ToKebabCase()));
            var routePrefix = configuration["WebApi:RoutePrefix"];
            if (!string.IsNullOrWhiteSpace(routePrefix))
            {
                options.Conventions.Add(new RoutePrefixConvention(new RouteAttribute(routePrefix)));
            }
        });
        builder.Services.AddMvcCore().AddApiExplorer().AddControllersAsServices()
            .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureDefault());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();

        #region OpenAPI document

        #region V1

        builder.Services.AddOpenApiDocument(configureDocument ?? (document =>
        {
            #region Document base info

            document.DocumentName = "v1";
            document.Version = "v1";
            document.Title = "DoliteTemplate";
            document.Description = "Some description of your API";
            document.ApiGroupNames = ["1"];

            #endregion

            #region Security

            #region Bearer

            document.AddSecurity("Bearer", [],
                new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'"
                });
            document.OperationProcessors.Add(
                new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

            #endregion

            #endregion
        }));

        #endregion

        #endregion

        #region Versioning

        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        #endregion
    }

    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        var encryptHelper = new EncryptHelper(builder.Configuration["Key:Path"]);
        builder.Services.AddSingleton(encryptHelper);
        builder.Services.ConfigureAuthentication(encryptHelper.GetPublicKey("user"));
    }

    public static void ConfigureAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options => options.AutoSetPolicies());
    }

    public static void ConfigureLocalization(this WebApplicationBuilder builder)
    {
        builder.Services.AddLocalization();
    }
}