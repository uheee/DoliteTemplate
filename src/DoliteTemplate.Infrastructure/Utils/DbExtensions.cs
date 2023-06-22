using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using DoliteTemplate.Infrastructure.DbContexts;
using DoliteTemplate.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DoliteTemplate.Infrastructure.Utils;

public static class DbExtensions
{
    private static string? _firstAvailableConnectionString;

    public static string GetAvailableConnectionString(this IConfiguration configuration)
    {
        var connectionStrings = configuration.GetSection("ConnectionStrings").Get<string[]>();
        return connectionStrings switch
        {
            null => throw new Exception("Invalid connection strings structure"),
            {Length: 0} => throw new Exception("No connection string"),
            _ => _firstAvailableConnectionString ??= connectionStrings.FirstOrDefault(connectionString =>
            {
                Log.Verbose("Attempt to connect to database using {ConnectionString}", connectionString);
                var dbContext = new ApiDbContext(connectionString);
                var canConnect = dbContext.Database.CanConnect();
                Log.Verbose("Connection with {ConnectionString} {Result}", connectionString,
                    canConnect ? "succeeded" : "failed");
                return canConnect;
            }) ?? throw new Exception("No available connection string")
        };
    }

    public static TDbContext GetDbContext<TDbContext>(this DbConnection connection) where TDbContext : DbContext
    {
        return typeof(TDbContext) switch
        {
            var type when type == typeof(ApiDbContext) => (TDbContext)(object)new ApiDbContext(connection),
            _ => throw new ArgumentException()
        };
    }

    public static dynamic DbSet(this DbContext dbContext, Type type)
    {
        var setMethod = typeof(DbContext).GetMethods()
            .Where(method => method.Name == nameof(DbContext.Set))
            .Single(method => !method.GetParameters().Any());
        setMethod = setMethod.MakeGenericMethod(type);
        return setMethod.Invoke(dbContext, null)!;
    }

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
        return (bool)anyMethod.Invoke(null, new[] {queryable})!;
    }

    public static bool HasDataWithPrimaryKeys<TEntity>(this DbContext dbContext, params object[] keyValues)
        where TEntity : class
    {
        var dbSet = dbContext.Set<TEntity>();
        return dbSet.Find(keyValues) is not null;
    }

    public static bool HasDataWithSpecifiedKeys<TEntity>(this DbContext dbContext,
        (ParameterExpression, IDictionary<ExpressionTuple, object?>) dict,
        TEntity? skipEntity) where TEntity : class
    {
        var (parameterExpression, keyValues) = dict;

        if (!keyValues.Any())
        {
            return false;
        }

        IQueryable<TEntity> dbSet = dbContext.Set<TEntity>();
        var expression = keyValues
            .Select(pair =>
            {
                var propertyExp = pair.Key;
                var valueExp = Expression.Constant(pair.Value);
                var equalExp = Expression.Equal(propertyExp.Member, valueExp);
                return Expression.Lambda<Func<TEntity, bool>>(equalExp, parameterExpression);
            })
            .Aggregate((a, b) =>
                Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(a.Body, b.Body), parameterExpression));

        if (skipEntity is not null)
        {
            dbSet = dbSet.Where(entity => entity != skipEntity);
        }

        return dbSet.Any(expression);
    }

    public static object[] GetPrimaryKeyValue(object entity, Type type, IKey primaryKey)
    {
        var properties = primaryKey.Properties
            .Select(property => property.Name)
            .Select(type.GetProperty);
        return properties.Select(property => property!.GetValue(entity)!).ToArray();
    }

    public static (ParameterExpression, IDictionary<ExpressionTuple, object?>)
        GetEntityTupleDict<TEntity>(TEntity entity, params string[] keys)
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity));
        var getters = keys
            .Select(key => GetNestedPropertyValueGetter<TEntity>(parameterExpression, key));
        return (parameterExpression,
            getters.ToDictionary(getter => getter,
                getter => getter.Expression.Compile().DynamicInvoke(entity)));
    }

    public static (ParameterExpression, IDictionary<ExpressionTuple, object?>)
        GetEntityTupleDictByEntry<TEntity>(EntityEntry entry, params string[] keys)
        where TEntity : class
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity));
        var getters = keys
            .Where(key =>
                entry.State == EntityState.Added ||
                (entry.State == EntityState.Modified && entry.Property(key).IsModified))
            .Select(key => GetNestedPropertyValueGetter<TEntity>(parameterExpression, key));
        return (parameterExpression,
            getters.ToDictionary(getter => getter,
                getter => getter.Expression.Compile().DynamicInvoke(entry.Entity)));
    }

    private static ExpressionTuple GetNestedPropertyValueGetter<TEntity>(
        ParameterExpression parameterExpression, string key)
    {
        var propertyNames = key.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Expression expression = parameterExpression;
        expression = propertyNames.Aggregate(expression, Expression.Property);
        var memberExpression = (MemberExpression)expression;
        var memberType = memberExpression.Type!;
        var funcType = typeof(Func<,>).MakeGenericType(typeof(TEntity), memberType);
        var lambda = Expression.Lambda(funcType, memberExpression, parameterExpression);
        return new ExpressionTuple(memberExpression, lambda);
    }

    public static Predicate<object> GetFilter(this DbContext dbContext, DbFilter filter, IEntityType entityType)
    {
        var clrType = entityType.ClrType;
        var primaryKey = entityType.FindPrimaryKey();
        var specifiedKeyValueGetterMethod = typeof(DbExtensions).GetMethod(nameof(GetEntityTupleDict))!
            .MakeGenericMethod(clrType);
        return entity => filter switch
        {
            {Mode: DbFilterMode.Always} => false,
            {Mode: DbFilterMode.Key, Args: null or {Length: 0}} when primaryKey is not null =>
                (bool)typeof(DbExtensions).GetMethod(nameof(HasDataWithPrimaryKeys),
                    BindingFlags.Public | BindingFlags.Static)!.MakeGenericMethod(clrType).Invoke(null,
                    new object[] {dbContext, GetPrimaryKeyValue(entity, clrType, primaryKey)})!,
            {Mode: DbFilterMode.Key, Args: {Length: > 0} key} =>
                (bool)typeof(DbExtensions).GetMethod(nameof(HasDataWithSpecifiedKeys),
                    BindingFlags.Public | BindingFlags.Static)!.MakeGenericMethod(clrType).Invoke(null,
                    new[]
                    {
                        dbContext, specifiedKeyValueGetterMethod.Invoke(null,
                            new[]
                            {
                                entity, key.Split(Strings.Separator,
                                    StringSplitOptions.TrimEntries |
                                    StringSplitOptions.RemoveEmptyEntries).ToArray()
                            })!,
                        null
                    })!,
            {Mode: DbFilterMode.Empty} => dbContext.HasData(clrType),
            _ => true
        };
    }
}