using System.Reflection;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureWebApi();
builder.ConfigureDbContext<ApiDbContext>(
    connectionString => new NpgsqlDataSourceBuilder(connectionString).EnableDynamicJson().Build(),
    (options, dataSource) => options.UseNpgsql(dataSource));
builder.ConfigureRedis("Redis");
builder.ConfigureAuthentication();
builder.ConfigureAuthorization();
builder.ConfigureLocalization();
builder.ConfigureSerilog();
Assembly[] assemblies =
[
    Assembly.GetExecutingAssembly(),
    Assembly.Load("DoliteTemplate.Api.Shared")
];
builder.ConfigureAutofac(assemblies: assemblies);
builder.ConfigureAutoMapper(assemblies);

var app = builder.Build();

app.UseLocalization();

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