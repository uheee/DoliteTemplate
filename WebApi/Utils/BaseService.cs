using AutoMapper;
using Domain.Utils;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebApi.Utils.Error;

namespace WebApi.Utils;

public class BaseService
{
    public IServiceProvider ServiceProvider { get; init; } = null!;
    public IMapper Mapper { get; init; } = null!;
    public DbContextOptions<ApiDbContext> DbContextOptions { get; init; } = null!;
    public ExceptionFactory ExceptionFactory { get; init; } = null!;
    public ApiDbContext DbContext { get; init; } = null!;
    protected ApiDbContext CreateDbContext() => new(DbContextOptions);
}