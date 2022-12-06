using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Infrastructure.Utils;

public static class LinqExtensions
{
    public static PaginatedList<TEntity> ToPagedList<TEntity>(this IQueryable<TEntity> queryable, int pageIndex,
        int pageSize)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;
        var count = queryable.LongCount();
        var items = queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize)
            .ToArray();
        return new PaginatedList<TEntity>(items, count, pageIndex, pageSize);
    }

    public static async Task<PaginatedList<TEntity>> ToPagedListAsync<TEntity>(this IQueryable<TEntity> queryable,
        int pageIndex, int pageSize)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;
        var count = await queryable.LongCountAsync();
        var items = await queryable.Skip(pageSize * (pageIndex - 1)).Take(pageSize)
            .ToArrayAsync();
        return new PaginatedList<TEntity>(items, count, pageIndex, pageSize);
    }

    public static IQueryable<TEntity> SkipDeleted<TEntity>(this IQueryable<TEntity> query)
    {
        return typeof(TEntity).IsAssignableTo(typeof(ISoftDelete))
            ? query.Where(entity => !((ISoftDelete)entity!).IsDeleted)
            : query;
    }

    public static IQueryable<TEntity> Query<TEntity>(this IQueryable<TEntity> query, QueryOptions<TEntity> options)
    {
        options.Predicates.ForEach(predicate => query = query.Where(predicate));
        options.OrderSelectors.ForEach(tuple =>
        {
            var (selector, type) = tuple;
            query = type switch
            {
                OrderType.Asc => query is IOrderedQueryable<TEntity> orderedQuery
                    ? orderedQuery.ThenBy(selector)
                    : query.OrderBy(selector),
                OrderType.Desc => query is IOrderedQueryable<TEntity> orderedQuery
                    ? orderedQuery.ThenByDescending(selector)
                    : query.OrderByDescending(selector),
                _ => throw new ArgumentOutOfRangeException()
            };
        });
        return query;
    }
}