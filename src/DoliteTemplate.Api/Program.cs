using System.Reflection;
using Asp.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DoliteTemplate.Api.Shared.Constants;
using DoliteTemplate.Domain.Shared.Utils;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Serilog

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

#endregion

#region Autofac

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(options =>
{
    options.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
});

#endregion

#region AutoMapper

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

#endregion

#region EntityFramework

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString(ConnectionStrings.Database);
    options.UseNpgsql(connectionString);
});

#endregion

#region Authentication

var encryptHelper = new EncryptHelper(builder.Configuration["Key:Path"]);
builder.Services.AddSingleton(encryptHelper);
builder.Services.ConfigureAuthentication(encryptHelper.GetPublicKey("user"));

#endregion

#region Authorization

builder.Services.AddAuthorization(options => options.AutoSetPolicies(Assembly.GetExecutingAssembly()));

#endregion

#region Localization

builder.Services.AddLocalization();

#endregion

#region Web API configuration

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new RouteKebabCaseTransformer()));
    options.Conventions.Add(new RoutePrefixConvention(new RouteAttribute("api/v{version:apiVersion}")));
});
builder.Services.AddMvcCore().AddApiExplorer().AddControllersAsServices()
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureDefault());
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

#region OpenAPI document

#region V1

builder.Services.AddOpenApiDocument(document =>
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
});

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

#endregion

var app = World.App = builder.Build();

app.UseLocalization(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();