using DoliteTemplate.DbMigrator;
using DoliteTemplate.DbMigrator.Seeding;
using DoliteTemplate.Infrastructure.DbContexts;
using DoliteTemplate.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString(ConnectionStrings.Database);
        services.AddDbContextFactory<ApiDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.MigrationsAssembly("DoliteTemplate.Infrastructure")));
        services.AddHostedService<DbMigrationService>();
        services.AddTransient<DataSeeder>();
    })
    .UseSerilog((_, configuration) => configuration.MinimumLevel.Information().WriteTo.Console())
    .Build();

await host.RunAsync();