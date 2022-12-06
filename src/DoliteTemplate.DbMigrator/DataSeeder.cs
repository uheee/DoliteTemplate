using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DoliteTemplate.DbMigrator;

public class DataSeeder
{
    private readonly IConfiguration _configuration;

    public DataSeeder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Seed(DbContext dbContext)
    {
        var assembly = Assembly.Load("DoliteTemplate.Domain");
        var dataSeeds = _configuration.GetSection("DataSeeds");
        using var transaction = dbContext.Database.BeginTransaction();
        foreach (var table in dataSeeds.GetChildren())
        {
            var typename = table.Key;
            var type = assembly.GetExportedTypes().SingleOrDefault(t => t.FullName!.EndsWith(typename));
            if (type is null)
            {
                Log.Error("Unable to find type {Type}", typename);
                continue;
            }

            if (dbContext.HasData(type))
            {
                Log.Warning("{Table} table has data, so seeding is skipped", typename);
                continue;
            }

            foreach (var item in table.GetChildren())
            {
                var entity = item.Get(type);
                if (entity is null)
                {
                    Log.Error("Some entity structure of {Type} is invalid", typename);
                    continue;
                }

                dbContext.Add(entity);
                dbContext.SaveChanges();
            }
        }

        transaction.Commit();
    }
}

public static class DbContextNonGenericExtensions
{
    public static bool HasData(this DbContext context, Type type)
    {
        var setMethod = typeof(DbContext).GetMethods()
            .Where(method => method.Name == nameof(DbContext.Set))
            .Single(method => !method.GetParameters().Any());
        setMethod = setMethod.MakeGenericMethod(type);
        var queryable = setMethod.Invoke(context, null);
        var anyMethod = typeof(Queryable).GetMethods()
            .Where(method => method.Name == nameof(Queryable.Any))
            .Single(method => method.GetParameters().Length == 1);
        anyMethod = anyMethod!.MakeGenericMethod(type);
        return (bool)anyMethod.Invoke(null, new[] { queryable })!;
    }
}