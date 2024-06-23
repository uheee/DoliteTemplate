using DoliteTemplate.Domain.Entities;
using DoliteTemplate.Infrastructure.DbContexts.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DoliteTemplate.Infrastructure.DbContexts;

public class ApiDbContext : BaseDbContext<ApiDbContext>
{
    #region Entites

    public DbSet<Order> Orders { get; init; }

    #endregion

    #region Constructors

    [ActivatorUtilitiesConstructor]
    public ApiDbContext(DbContextOptions<ApiDbContext> options) :
        base(options)
    {
    }

    public ApiDbContext(DbContextOptions<ApiDbContext> options, IHttpContextAccessor httpContextAccessor) :
        base(options, httpContextAccessor)
    {
    }

    #endregion
}