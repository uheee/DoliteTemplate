using AutoMapper;
using Domain.Utils;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using WebApi.Utils.Error;

namespace WebApi.Utils;

public class BaseService
{
    public IMapper Mapper { get; init; } = null!;
    public ExceptionFactory ExceptionFactory { get; init; } = null!;
    public ApiDbContext DbContext { get; init; } = null!;
    public TransactionDbContextProvider DbContextProvider { get; init; } = null!;

    protected Task UseTransaction(Func<ApiDbContext, Task> action)
    {
        return UseTransaction(async provider => await action(await provider.GetDbContext()));
    }

    protected Task<T> UseTransaction<T>(Func<ApiDbContext, Task<T>> action)
    {
        return UseTransaction(async provider => await action(await provider.GetDbContext()));
    }

    protected async Task UseTransaction(Func<TransactionDbContextProvider, Task> action)
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

    protected async Task<T> UseTransaction<T>(Func<TransactionDbContextProvider, Task<T>> action)
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

    protected async Task<PagedList<T>> PagingQuery<T>(Func<ApiDbContext, IQueryable<T>> query, int index, int pageSize)
    {
        if (index < 1) return PagedList<T>.Empty(index, pageSize);
        return await UseTransaction(async provider =>
        {
            var itemCount = query(await provider.GetDbContext()).LongCountAsync();
            var items = query(await provider.GetDbContext()).Skip(pageSize * (index - 1)).Take(pageSize)
                .ToArrayAsync();
            await Task.WhenAll(itemCount, items);
            return new PagedList<T>(await items, await itemCount, index, pageSize);
        });
    }
}