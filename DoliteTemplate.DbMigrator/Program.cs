using DoliteTemplate.DbMigrator;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Default")!;
        services.AddDbContextFactory<ApiDbContext>(options =>
            options.UseNpgsql(connectionString, o => o.MigrationsAssembly(nameof(Infrastructure))));
        services.AddHostedService<DbMigrationService>();
    })
    .Build();

await host.RunAsync();