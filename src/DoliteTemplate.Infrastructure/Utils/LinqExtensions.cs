using System.Linq.Expressions;
using DoliteTemplate.Domain.Entities.Base;
using DoliteTemplate.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Infrastructure.Utils;

public static class LinqExtensions
{
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

    public static IQueryable<TEntity> SkipDeleted<TEntity>(this IQueryable<TEntity> query)
    {
        return typeof(TEntity).IsAssignableTo(typeof(ISoftDeletable))
            ? query.Where(entity => !((ISoftDeletable)entity!).IsDeleted)
            : query;
    }

    public static IQueryable<TEntity> WhereIf<TEntity>(
        this IQueryable<TEntity> query, bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IEnumerable<TEntity> WhereIf<TEntity>(
        this IEnumerable<TEntity> query, bool condition, Func<TEntity, bool> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}