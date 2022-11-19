using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Infrastructure.Utils;

public static class LinqExtensions
{
    public static PagedList<TEntity> ToPagedList<TEntity>(this IQueryable<TEntity> queryable, int index, int pageSize)
    {
        if (index < 1) return PagedList<TEntity>.Empty(index, pageSize);
        var itemCount = queryable.LongCount();
        var items = queryable.Skip(pageSize * (index - 1)).Take(pageSize)
            .ToArray();
        return new PagedList<TEntity>(items, itemCount, index, pageSize);
    }

    public static async Task<PagedList<TEntity>> ToPagedListAsync<TEntity>(this IQueryable<TEntity> queryable,
        int index, int pageSize)
    {
        if (index < 1) return PagedList<TEntity>.Empty(index, pageSize);
        var itemCount = await queryable.LongCountAsync();
        var items = await queryable.Skip(pageSize * (index - 1)).Take(pageSize)
            .ToArrayAsync();
        return new PagedList<TEntity>(items, itemCount, index, pageSize);
    }

    public static IQueryable<TEntity> SkipDeleted<TEntity>(this IQueryable<TEntity> query)
    {
        return typeof(TEntity).IsAssignableTo(typeof(ISoftDelete))
            ? query.Where(entity => !((ISoftDelete) entity!).IsDeleted)
            : query;
    }

    public static IQueryable<TEntity> QueryBy<TEntity>(this IQueryable<TEntity> query, QueryOptions<TEntity> options)
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