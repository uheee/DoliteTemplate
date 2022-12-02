using AutoMapper;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Api.Utils.Error;
using DoliteTemplate.Domain.Services.Base;
using DoliteTemplate.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Services.Base;

public class BaseService
{
    public IMapper Mapper { get; init; } = null!;
    public ExceptionFactory ExceptionFactory { get; init; } = null!;
}

public class BaseService<TDbContext> : BaseService, IBaseService where TDbContext : DbContext
{
    public TDbContext DbContext { get; init; } = null!;
    public DbContextProvider<TDbContext> DbContextProvider { get; init; } = null!;

    protected Task UseTransaction(Func<TDbContext, Task> action)
    {
        return UseTransaction(async provider => await action(await provider.GetDbContext()));
    }

    protected Task<TResult> UseTransaction<TResult>(Func<TDbContext, Task<TResult>> action)
    {
        return UseTransaction(async provider => await action(await provider.GetDbContext()));
    }

    protected async Task UseTransaction(Func<DbContextProvider<TDbContext>, Task> action)
    {
        try
        {
            await action(DbContextProvider);
            if (DbContextProvider.Transaction is null)
                throw new Exception("DbContext count is less than 1");
            await DbContextProvider.Transaction.CommitAsync();
        }
        catch (Exception e)
        {
            if (DbContextProvider.Transaction is not null)
                await DbContextProvider.Transaction.RollbackAsync();
            throw;
        }
    }

    protected async Task<TResult> UseTransaction<TResult>(Func<DbContextProvider<TDbContext>, Task<TResult>> action)
    {
        try
        {
            var result = await action(DbContextProvider);
            if (DbContextProvider.Transaction is null)
                throw new Exception("DbContext count is less than 1");
            await DbContextProvider.Transaction.CommitAsync();
            return result;
        }
        catch (Exception e)
        {
            if (DbContextProvider.Transaction is not null)
                await DbContextProvider.Transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PaginatedList<TEntity>> PagingQuery<TEntity>(Func<TDbContext, IQueryable<TEntity>> query,
        int pageIndex, int pageSize)
    {
        if (pageIndex < 1) return PaginatedList<TEntity>.Empty(pageIndex, pageSize);
        return await UseTransaction(async provider =>
        {
            var count = query(await provider.GetDbContext()).LongCountAsync();
            var items = query(await provider.GetDbContext()).Skip(pageSize * (pageIndex - 1)).Take(pageSize)
                .ToArrayAsync();
            await Task.WhenAll(count, items);
            return new PaginatedList<TEntity>(await items, await count, pageIndex, pageSize);
        });
    }
}