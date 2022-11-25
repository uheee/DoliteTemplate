using DoliteTemplate.DbMigrator;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Default")!;
        services.AddDbContextFactory<ApiDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.MigrationsAssembly("DoliteTemplate.Infrastructure")));
        services.AddHostedService<DbMigrationService>();
    })
    .UseSerilog((_, configuration) => configuration.MinimumLevel.Information().WriteTo.Console())
    .Build();

await host.RunAsync();