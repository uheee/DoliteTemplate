using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Shared.Utils;
using GraphQL;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Use Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(options =>
    options.RegisterAssemblyModules(Assembly.GetExecutingAssembly()));

// Use AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddGraphQL(b =>
{
    b.AddAutoSchema<Query>();
    b.AddSystemTextJson(options =>
    {
        options.WriteIndented = true;
    });
});

// Use EF Core
// builder.Services.AddDbContext<ApiDbContext>();

// Use Auth
var encryptHelper = new EncryptHelper(builder.Configuration["Key:Path"]);
builder.Services.AddSingleton(encryptHelper);
builder.Services.ConfigureAuthentication(builder.Configuration, encryptHelper.GetPublicKey("user"));
builder.Services.AddAuthorization(options => options.AutoSetPolicies());

// Use Localization
builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddMvcCore().AddApiExplorer().AddControllersAsServices()
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureDefault());
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOpenApi(builder.Configuration);

var app = World.App = builder.Build();

// Globalization & Localization
app.UseLocalization(builder.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseExceptionHandler();

// Configure RESTful API
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configure GraphQL
app.UseGraphQL();
app.UseGraphQLPlayground("/");

await app.RunAsync();

public class Query
{
    public static string Hero()
    {
        return "Luke Skywalker";
    }
}