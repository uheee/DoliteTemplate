using DoliteTemplate.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DoliteTemplate.DbMigrator.Seeding;

/// <summary>
///     数据播种器
/// </summary>
public class DataSeeder
{
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     构造数据播种器
    /// </summary>
    /// <param name="configuration">配置项</param>
    public DataSeeder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    ///     播种
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    public void Seed(DbContext dbContext)
    {
        var dataSeeds = _configuration.GetSection("DataSeeds");
        using var transaction = dbContext.Database.BeginTransaction();
        foreach (var seedSection in dataSeeds.GetChildren())
        {
            var seed = seedSection.Get<Seed>();
            if (seed is null || !string.Equals(seed.DbContext, dbContext.GetType().Name,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            Log.Information("Seeding [{Tag}]", seed.Tag);

            var typename = seed.Entity;
            var entityType = dbContext.Model.GetEntityTypes()
                .SingleOrDefault(type => type.Name!.EndsWith(typename));
            if (entityType is null)
            {
                Log.Error("Unable to find entity type {Type}", typename);
                continue;
            }

            var filter = dbContext.GetFilter(seed.Filter, entityType);
            foreach (var entity in seed.Data.GetEntities(entityType.ClrType))
            {
                if (filter(entity))
                {
                    Log.Information("Trigger {@Trigger} is not satisfied, seed will be skipped", seed.Filter);
                    continue;
                }

                dbContext.Add(entity);
                dbContext.SaveChanges();
            }
        }

        transaction.Commit();
    }
}