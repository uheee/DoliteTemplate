using System.Data.Common;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Utils;

public class TransactionDbContextProvider : IDisposable
{
    private readonly List<DbContext> _dbContexts = new();
    public DbConnection DbConnection { get; init; } = null!;
    public DbTransaction Transaction { get; init; } = null!;

    public void Dispose()
    {
        _dbContexts.ForEach(dbContext => dbContext.Dispose());
    }


    public async Task<ApiDbContext> GetDbContext()
    {
        var dbContext = new ApiDbContext(DbConnection);
        await dbContext.Database.UseTransactionAsync(Transaction);
        _dbContexts.Add(dbContext);
        return dbContext;
    }
}