using System.Data.Common;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Utils;

public class DbContextProvider<TDbContext> : IDisposable where TDbContext : DbContext
{
    private readonly List<TDbContext> _dbContexts = new();
    public DbConnection DbConnection { get; init; } = null!;
    public DbTransaction Transaction { get; init; } = null!;

    public void Dispose()
    {
        _dbContexts.ForEach(dbContext => dbContext.Dispose());
    }


    public async Task<TDbContext> GetDbContext()
    {
        var dbContext = DbConnection.GetDbContext<TDbContext>();
        await dbContext.Database.UseTransactionAsync(Transaction);
        _dbContexts.Add(dbContext);
        return dbContext;
    }
}