using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using WebApi;
using WebApi.Utils;
using WebApi.Utils.Autofac;
using WebApi.Utils.AutoMapper;
using WebApi.Utils.Error;

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Use Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(options =>
    options.RegisterModule(new BaseModule(builder.Configuration)));

// Use AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Use EF Core
builder.Services.AddDbContext<ApiDbContext>();

// Use Auth
var encryptHelper = new EncryptHelper(GlobalDefinitions.KeyPath);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters =
        GlobalDefinitions.JwtBearerTokenValidationParameters(encryptHelper.GetPublicKey("user")));
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddMvcCore().AddApiExplorer().AddControllersAsServices().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var info = GlobalDefinitions.OpenApiInfo;
    options.SwaggerDoc(info.Version, info);
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, GlobalDefinitions.OpenApiSecurityScheme);
    options.AddSecurityRequirement(GlobalDefinitions.OpenApiSecurityRequirement);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()!.Error;
        var error = exception.ToErrorInfo(app.Environment.IsDevelopment());
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = exception switch
        {
            BusinessException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        var jsonOptions = app.Services.GetService<IOptions<JsonOptions>>()!.Value;
        await context.Response.WriteAsJsonAsync(error, jsonOptions.JsonSerializerOptions);
    });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();