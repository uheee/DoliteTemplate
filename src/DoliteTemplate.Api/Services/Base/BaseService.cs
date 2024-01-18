using System.Data;
using AutoMapper;
using DoliteTemplate.Api.Utils;
using DoliteTemplate.Api.Utils.Error;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Npgsql;
using Serilog;
using StackExchange.Redis;

namespace DoliteTemplate.Api.Services.Base;

public abstract class BaseService(IMapper mapper)
{
    public IMapper Mapper => mapper;
}

public class BaseService<TService>(
    IMapper mapper,
    IConnectionMultiplexer redis,
    IHttpContextAccessor httpContextAccessor,
    EncryptHelper encryptHelper,
    IStringLocalizer<TService> localizer) : BaseService(mapper), ICulturalResource
    where TService : BaseService<TService>
{
    public BusinessException Error(string errCode, params object[] args)
    {
        var errTemplate = localizer[errCode];
        var errMsg = string.Format(errTemplate, args);
        if (string.IsNullOrEmpty(errMsg))
        {
            errMsg = "unknown";
        }

        return new BusinessException(errCode, errMsg);
    }
}

public class BaseService<TService, TDbContext>(
    IMapper mapper,
    IConnectionMultiplexer redis,
    IHttpContextAccessor httpContextAccessor,
    EncryptHelper encryptHelper,
    IStringLocalizer<TService> localizer,
    NpgsqlConnection dbConnection,
    Lazy<TDbContext> dbContextLazier,
    Lazy<DbContextProvider<TDbContext>> dbContextProviderLazier)
    : BaseService<TService>(mapper, redis, httpContextAccessor, encryptHelper, localizer)
    where TService : BaseService<TService, TDbContext>
    where TDbContext : DbContext
{
    public TDbContext DbContext => dbContextLazier.Value;
public DbContextProvider<TDbContext> DbContextProvider => dbContextProviderLazier.Value;

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
        {
            throw new Exception("DbContext count is less than 1");
        }

        await DbContextProvider.Transaction.CommitAsync();
    }
    catch (Exception e)
    {
        Log.Error(e, "Failed to execute transaction");
        if (DbContextProvider.Transaction is not null)
        {
            await DbContextProvider.Transaction.RollbackAsync();
        }

        throw;
    }
}

protected async Task<TResult> UseTransaction<TResult>(Func<DbContextProvider<TDbContext>, Task<TResult>> action)
{
    try
    {
        var result = await action(DbContextProvider);
        if (DbContextProvider.Transaction is null)
        {
            throw new Exception("DbContext count is less than 1");
        }

        await DbContextProvider.Transaction.CommitAsync();
        return result;
    }
    catch (Exception e)
    {
        Log.Error(e, "Failed to execute transaction");
        if (DbContextProvider.Transaction is not null)
        {
            await DbContextProvider.Transaction.RollbackAsync();
        }

        throw;
    }
}

#endregion

#region Raw SQL execution

public async Task<IDataReader> RawSqlQuery(string statement, params object[] parameters)
{
    await using var command = dbConnection.CreateCommand();
    command.CommandText = statement;
    command.Parameters.AddRange(parameters);
    return await command.ExecuteReaderAsync();
}

public async Task<int> RawSqlNonQuery(string statement, params object[] parameters)
{
    await using var command = dbConnection.CreateCommand();
    command.CommandText = statement;
    command.Parameters.AddRange(parameters);
    return await command.ExecuteNonQueryAsync();
}

    #endregion
}