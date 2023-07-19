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

public abstract class BaseService
{
    public IMapper Mapper { get; init; } = null!;
}

public class BaseService<TService> : BaseService, ICulturalResource<TService>
    where TService : BaseService<TService>
{
    public IConnectionMultiplexer Redis { get; init; } = null!;
    public IHttpContextAccessor HttpContextAccessor { get; init; } = null!;
    public EncryptHelper EncryptHelper { get; init; } = null!;
    public IStringLocalizer<TService> Localizer { get; init; } = null!;

    public BusinessException Error(string errCode, params object[] args)
    {
        var errTemplate = Localizer[errCode];
        var errMsg = string.Format(errTemplate, args);
        if (string.IsNullOrEmpty(errMsg))
        {
            errMsg = "unknown";
        }

        return new BusinessException(errCode, errMsg);
    }
}

public class BaseService<TService, TDbContext> : BaseService<TService>
    where TService : BaseService<TService, TDbContext>
    where TDbContext : DbContext
{
    public NpgsqlConnection DbConnection { get; init; } = null!;
    public Lazy<TDbContext> DbContextLazier { get; init; } = null!;
    public TDbContext DbContext => DbContextLazier.Value;
    public Lazy<DbContextProvider<TDbContext>> DbContextProviderLazier { get; init; } = null!;
    public DbContextProvider<TDbContext> DbContextProvider => DbContextProviderLazier.Value;
    public NpgsqlLargeObjectManager ObjectManager { get; init; } = null!;

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
        await using var command = DbConnection.CreateCommand();
        command.CommandText = statement;
        command.Parameters.AddRange(parameters);
        return await command.ExecuteReaderAsync();
    }

    public async Task<int> RawSqlNonQuery(string statement, params object[] parameters)
    {
        await using var command = DbConnection.CreateCommand();
        command.CommandText = statement;
        command.Parameters.AddRange(parameters);
        return await command.ExecuteNonQueryAsync();
    }

    #endregion
}