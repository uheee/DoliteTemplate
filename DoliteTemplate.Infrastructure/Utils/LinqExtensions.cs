using DoliteTemplate.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Infrastructure.Utils;

public static class LinqExtensions
{
    public static PagedList<T> ToPagedList<T>(this IQueryable<T> queryable, int index, int pageSize)
    {
        if (index < 1) return PagedList<T>.Empty(index, pageSize);
        var itemCount = queryable.LongCount();
        var items = queryable.Skip(pageSize * (index - 1)).Take(pageSize)
            .ToArray();
        return new PagedList<T>(items, itemCount, index, pageSize);
    }

    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> queryable, int index, int pageSize)
    {
        if (index < 1) return PagedList<T>.Empty(index, pageSize);
        var itemCount = await queryable.LongCountAsync();
        var items = await queryable.Skip(pageSize * (index - 1)).Take(pageSize)
            .ToArrayAsync();
        return new PagedList<T>(items, itemCount, index, pageSize);
    }
}