using DoliteTemplate.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DoliteTemplate.DbMigrator.Seeding;

public class DataSeeder
{
    private readonly IConfiguration _configuration;

    public DataSeeder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Seed(DbContext dbContext)
    {
        var dataSeeds = _configuration.GetSection("DataSeeds");
        using var transaction = dbContext.Database.BeginTransaction();
        foreach (var seedSection in dataSeeds.GetChildren())
        {
            var seed = seedSection.Get<Seed>();
            if (seed is null)
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