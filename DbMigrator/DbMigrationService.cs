using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DbMigrator;

public class DbMigrationService : BackgroundService
{
    private readonly IDbContextFactory<ApiDbContext> _dbContextFactory;
    private readonly ILogger<DbMigrationService> _logger;

    public DbMigrationService(ILogger<DbMigrationService> logger,
        IDbContextFactory<ApiDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
        await dbContext.Database.MigrateAsync(stoppingToken);
    }
}