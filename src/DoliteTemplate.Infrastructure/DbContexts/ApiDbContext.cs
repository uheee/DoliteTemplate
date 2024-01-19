using DoliteTemplate.Infrastructure.DbContexts.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DoliteTemplate.Infrastructure.DbContexts;

public class ApiDbContext : BaseDbContext<ApiDbContext>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) :
        base(options)
    {
    }

    public ApiDbContext(DbContextOptions<ApiDbContext> options, IHttpContextAccessor httpContextAccessor) :
        base(options, httpContextAccessor)
    {
    }
}