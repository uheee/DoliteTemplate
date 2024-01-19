using DoliteTemplate.Api.Shared.Constants;
using DoliteTemplate.DbMigrator;
using DoliteTemplate.DbMigrator.Seeding;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

const string infrastructureNamespace = "DoliteTemplate.Infrastructure";
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        ConfigureApiDbContext(context, services);
        services.AddTransient<DataSeeder>();
    })
    .UseSerilog((_, configuration) => configuration.MinimumLevel.Information().WriteTo.Console())
    .Build();

await host.RunAsync();
return;

void ConfigureApiDbContext(HostBuilderContext context, IServiceCollection services)
{
    var connectionString = context.Configuration.GetConnectionString(ConnectionStrings.Database);
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception($"Missing {nameof(ApiDbContext)} connection string");
    }

    services.AddDbContextFactory<ApiDbContext>(options =>
        options.UseNpgsql(connectionString,
            npgsql => npgsql.MigrationsAssembly(infrastructureNamespace)));
    services.AddHostedService<DbMigrationService<ApiDbContext>>();
}