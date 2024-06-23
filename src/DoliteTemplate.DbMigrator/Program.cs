using DoliteTemplate.Api.Shared.Constants;
using DoliteTemplate.Api.Shared.Utils;
using DoliteTemplate.DbMigrator;
using DoliteTemplate.DbMigrator.Seeding;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

const string infrastructureNamespace = "DoliteTemplate.Infrastructure";
var builder = Host.CreateApplicationBuilder(args);

#region Serilog

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(new LoggerConfiguration().MinimumLevel.Information().WriteTo.Console().CreateLogger());

#endregion

#region DbContext

builder.ConfigureDbContextFactory<ApiDbContext>(
    connectionString => new NpgsqlDataSourceBuilder(connectionString).EnableDynamicJson().Build(),
    (options, dataSource) => options.UseNpgsql(dataSource));
builder.Services.AddHostedService<DbMigrationService<ApiDbContext>>();

#endregion

#region Seeder

builder.Services.AddTransient<DataSeeder>();

#endregion

var host = builder.Build();

await host.RunAsync();