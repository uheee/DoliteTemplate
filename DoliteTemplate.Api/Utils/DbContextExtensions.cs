using System.Data.Common;
using DoliteTemplate.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Api.Utils;

public static class DbContextExtensions
{
    public static TDbContext GetDbContext<TDbContext>(this DbConnection connection) where TDbContext : DbContext
    {
        return typeof(TDbContext) switch
        {
            var type when type == typeof(ApiDbContext) => (TDbContext) (object) new ApiDbContext(connection),
            _ => throw new ArgumentException()
        };
    }
}