using System.Data.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DbContexts;

public class ApiDbContext : DbContext
{
    private readonly DbConnection? _connection;
    private readonly DbDataSource? _dataSource;

    public ApiDbContext(DbDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public ApiDbContext(DbConnection connection)
    {
        _connection = connection;
    }

    [ActivatorUtilitiesConstructor]
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (builder.IsConfigured) return;

        if (_connection is not null)
            builder.UseNpgsql(_connection);
        else if (_dataSource is not null) builder.UseNpgsql(_dataSource.ConnectionString);

        builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Device>().HasData(new Device
        {
            Type = "Electric",
            Name = "Computer"
        });
    }
}