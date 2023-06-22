using System.Runtime.CompilerServices;
using System.Text;
using DoliteTemplate.DbMigrator.Seeding;
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

        Log.Information("Executing SQL scripts...");
        var dirname = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");
        await foreach (var script in GetScripts(dirname, stoppingToken))
        {
            await dbContext.Database.ExecuteSqlRawAsync(script, stoppingToken);
        }

        Log.Information("Starting data seeding...");
        _dataSeeder.Seed(dbContext);

        Log.Information("Database migration complete");
        Environment.Exit(0);
    }

    private static async IAsyncEnumerable<string> GetScripts(string dirname,
        [EnumeratorCancellation] CancellationToken stoppingToken)
    {
        foreach (var file in Directory.GetFiles(dirname, "*.sql"))
        {
            var sql = await File.ReadAllTextAsync(file, Encoding.UTF8, stoppingToken);
            yield return sql;
        }
    }
}