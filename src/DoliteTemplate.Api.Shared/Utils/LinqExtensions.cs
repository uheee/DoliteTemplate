using System.Linq.Expressions;
using DoliteTemplate.Domain.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     LINQ表达式扩展
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    ///     转换为分页列表
    /// </summary>
    /// <param name="queryable">查询</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>分页列表</returns>
    public static PaginatedList<TEntity> ToPagedList<TEntity>(this IQueryable<TEntity> queryable, int pageIndex,
        int pageSize)
    {
        if (pageIndex < 1)
        {
            pageIndex = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        var count = queryable.LongCount();
        var items = queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize)
            .ToArray();
        return new PaginatedList<TEntity>(items, count, pageIndex, pageSize);
    }

    /// <summary>
    ///     转换为分页列表
    /// </summary>
    /// <param name="queryable">查询</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>分页列表</returns>
    public static async Task<PaginatedList<TEntity>> ToPagedListAsync<TEntity>(this IQueryable<TEntity> queryable,
        int pageIndex, int pageSize)
    {
        if (pageIndex < 1)
        {
            pageIndex = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        var count = await queryable.LongCountAsync();
        var items = await queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize)
            .ToArrayAsync();
        return new PaginatedList<TEntity>(items, count, pageIndex, pageSize);
    }

    /// <summary>
    ///     转换为分页列表
    /// </summary>
    /// <param name="enumerable">可枚举对象</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>分页列表</returns>
    public static PaginatedList<TEntity> ToPagedList<TEntity>(this IEnumerable<TEntity> enumerable,
        int pageIndex, int pageSize)
    {
        if (pageIndex < 1)
        {
            pageIndex = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        var count = enumerable.LongCount();
        var items = enumerable.Skip(pageSize * (pageIndex - 1)).Take(pageSize)
            .ToArray();
        return new PaginatedList<TEntity>(items, count, pageIndex, pageSize);
    }

    /// <summary>
    ///     当条件成立时附加Where查询
    /// </summary>
    /// <param name="query">查询</param>
    /// <param name="condition">条件</param>
    /// <param name="predicate">Where查询表达式</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>实体可枚举对象</returns>
    public static IQueryable<TEntity> WhereIf<TEntity>(
        this IQueryable<TEntity> query, bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    ///     当条件成立时附加Where查询
    /// </summary>
    /// <param name="query">查询</param>
    /// <param name="condition">条件</param>
    /// <param name="predicate">Where查询表达式</param>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>实体可枚举对象</returns>
    public static IEnumerable<TEntity> WhereIf<TEntity>(
        this IEnumerable<TEntity> query, bool condition, Func<TEntity, bool> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}