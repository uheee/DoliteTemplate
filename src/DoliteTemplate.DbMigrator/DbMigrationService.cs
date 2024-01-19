using System.Runtime.CompilerServices;
using System.Text;
using DoliteTemplate.DbMigrator.Seeding;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DoliteTemplate.DbMigrator;

/// <summary>
///     数据库迁移服务
/// </summary>
/// <typeparam name="TDbContext">数据库上下文类型</typeparam>
public class DbMigrationService<TDbContext> : BackgroundService where TDbContext : DbContext
{
    private readonly DataSeeder _dataSeeder;
    private readonly IDbContextFactory<TDbContext> _dbContextFactory;

    /// <summary>
    ///     构造数据库迁移服务
    /// </summary>
    /// <param name="dbContextFactory">数据库上下文工厂</param>
    /// <param name="dataSeeder">数据播种器</param>
    public DbMigrationService(IDbContextFactory<TDbContext> dbContextFactory, DataSeeder dataSeeder)
    {
        _dbContextFactory = dbContextFactory;
        _dataSeeder = dataSeeder;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Starting database migration for {DbContext}...", typeof(TDbContext));
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
        await dbContext.Database.MigrateAsync(stoppingToken);

        Log.Information("Executing SQL scripts...");
        var scriptDir = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");
        if (Directory.Exists(scriptDir))
        {
            await foreach (var script in GetScripts(scriptDir, stoppingToken))
            {
                await dbContext.Database.ExecuteSqlRawAsync(script, stoppingToken);
            }
        }

        Log.Information("Starting data seeding...");
        _dataSeeder.Seed(dbContext);

        Log.Information("Database migration complete");
        Environment.Exit(0);
    }

    /// <summary>
    ///     获取目录下的SQL脚本
    /// </summary>
    /// <param name="dirname">目录名称</param>
    /// <param name="stoppingToken">终止令牌</param>
    /// <returns></returns>
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