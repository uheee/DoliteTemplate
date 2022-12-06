using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DoliteTemplate.DbMigrator;

public class DbMigrationService : BackgroundService
{
    private readonly DataSeeder _dataSeeder;
    private readonly IDbContextFactory<ApiDbContext> _dbContextFactory;

    public DbMigrationService(IDbContextFactory<ApiDbContext> dbContextFactory, DataSeeder dataSeeder)
    {
        _dbContextFactory = dbContextFactory;
        _dataSeeder = dataSeeder;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Starting database migration...");
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
        await dbContext.Database.MigrateAsync(stoppingToken);
        Log.Information("Starting data seeding...");
        _dataSeeder.Seed(dbContext);
        Log.Information("Database migration complete");
        Environment.Exit(0);
    }
}