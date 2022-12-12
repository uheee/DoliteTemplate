using AutoMapper;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Api.Utils.Error;
using DoliteTemplate.Domain.Services.Base;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DoliteTemplate.Api.Services.Base;

public class BaseService
{
    public IMapper Mapper { get; init; } = null!;
    public ExceptionFactory ExceptionFactory { get; init; } = null!;
}

public class BaseService<TDbContext> : BaseService, IBaseService where TDbContext : DbContext
{
    public TDbContext DbContext { get; init; } = null!;
    public Lazy<DbContextProvider<TDbContext>> DbContextProviderLazier { get; init; } = null!;
    public DbContextProvider<TDbContext> DbContextProvider => DbContextProviderLazier.Value;

    #region Transaction

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
            Log.Error(e, "Failed to execute transaction");
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
            Log.Error(e, "Failed to execute transaction");
            if (DbContextProvider.Transaction is not null)
                await DbContextProvider.Transaction.RollbackAsync();
            throw;
        }
    }

    #endregion
}