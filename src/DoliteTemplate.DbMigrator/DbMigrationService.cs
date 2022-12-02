using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DoliteTemplate.DbMigrator;

public class DbMigrationService : BackgroundService
{
    private readonly IDbContextFactory<ApiDbContext> _dbContextFactory;

    public DbMigrationService(IDbContextFactory<ApiDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
        await dbContext.Database.MigrateAsync(stoppingToken);
        Log.Information("Database migration complete");
        Environment.Exit(0);
    }
}