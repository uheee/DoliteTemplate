using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper.Internal;
using DoliteTemplate.Domain.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DoliteTemplate.Infrastructure.Utils;

/// <summary>
///     数据库扩展
/// </summary>
public static class DbExtensions
{
    /// <summary>
    ///     查询是否存在指定类型的实体
    /// </summary>
    /// <param name="context">数据库上下文</param>
    /// <param name="type">实体类型</param>
    /// <returns>是否存在</returns>
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

    /// <summary>
    ///     查询是否存在指定主键的实体
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="keyValues">主键</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>是否存在</returns>
    public static bool HasDataWithPrimaryKeys<TEntity>(this DbContext dbContext, params object[] keyValues)
        where TEntity : class
    {
        var dbSet = dbContext.Set<TEntity>();
        return dbSet.Find(keyValues) is not null;
    }

    /// <summary>
    ///     查询是否存在指定键值的实体
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="dict">指定键值</param>
    /// <param name="skipEntity">需要跳过的实体</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>是否存在</returns>
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
                var memberExp = propertyExp.Member;
                if (memberExp.Type.IsValueType && memberExp.Type.IsNullableType() && pair.Value is not null)
                {
                    memberExp = Expression.Property(memberExp, nameof(Nullable<int>.Value));
                }

                var equalExp = Expression.Equal(memberExp, valueExp);
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

    /// <summary>
    ///     获取实体主键值
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="type">实体类型</param>
    /// <param name="primaryKey">主键</param>
    /// <returns>主键值</returns>
    public static object[] GetPrimaryKeyValue(object entity, Type type, IKey primaryKey)
    {
        var properties = primaryKey.Properties
            .Select(property => property.Name)
            .Select(type.GetProperty);
        return properties.Select(property => property!.GetValue(entity)!).ToArray();
    }

    /// <summary>
    ///     获取实体键值
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="keys">实体键</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>实体键值</returns>
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

    /// <summary>
    ///     基于Entry获取实体键值
    /// </summary>
    /// <param name="entry">实体</param>
    /// <param name="keys">实体键</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>实体键值</returns>
    public static (ParameterExpression, IDictionary<ExpressionTuple, object?>)
        GetEntityTupleDictByEntry<TEntity>(EntityEntry entry, params string[] keys)
        where TEntity : class
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity));
        var getters = keys
            // .Where(key => 
            //     entry.State == EntityState.Added ||
            //     (entry.State == EntityState.Modified && entry.Property(key).IsModified))
            .Select(key => GetNestedPropertyValueGetter<TEntity>(parameterExpression, key));
        return (parameterExpression,
            getters.ToDictionary(getter => getter,
                getter => getter.Expression.Compile().DynamicInvoke(entry.Entity)));
    }

    /// <summary>
    ///     获取实体嵌套属性
    /// </summary>
    /// <param name="parameterExpression">参数表达式</param>
    /// <param name="key">实体键</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns></returns>
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

    /// <summary>
    ///     获取数据库过滤器
    /// </summary>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="filter">过滤器描述</param>
    /// <param name="entityType">实体类型</param>
    /// <returns>数据库过滤器</returns>
    public static Predicate<object> GetFilter(this DbContext dbContext, DbFilter filter, IEntityType entityType)
    {
        var clrType = entityType.ClrType;
        var primaryKey = entityType.FindPrimaryKey();
        var specifiedKeyValueGetterMethod = typeof(DbExtensions).GetMethod(nameof(GetEntityTupleDict))!
            .MakeGenericMethod(clrType);
        return entity => filter switch
        {
            { Mode: DbFilterMode.Always } => false,
            { Mode: DbFilterMode.Key, Args: null or { Length: 0 } } when primaryKey is not null =>
                (bool)typeof(DbExtensions).GetMethod(nameof(HasDataWithPrimaryKeys),
                    BindingFlags.Public | BindingFlags.Static)!.MakeGenericMethod(clrType).Invoke(null,
                    new object[] { dbContext, GetPrimaryKeyValue(entity, clrType, primaryKey) })!,
            { Mode: DbFilterMode.Key, Args: { Length: > 0 } key } =>
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
            { Mode: DbFilterMode.Empty } => dbContext.HasData(clrType),
            _ => true
        };
    }

    /// <summary>
    ///     读取所有动态记录
    /// </summary>
    /// <param name="dataReader">数据库读取器</param>
    /// <returns>记录枚举</returns>
    public static IEnumerable<IDataRecord> ReadAll(this IDataReader dataReader)
    {
        while (dataReader.Read())
        {
            yield return dataReader;
        }
    }
}