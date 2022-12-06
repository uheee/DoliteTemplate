using System.Data.Common;
using DoliteTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DoliteTemplate.Infrastructure.DbContexts;

public class ApiDbContext : DbContext
{
    private readonly DbConnection? _connection;
    private readonly string? _connectionString;

    public ApiDbContext(DbConnection connection)
    {
        _connection = connection;
    }

    public ApiDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    [ActivatorUtilitiesConstructor]
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (builder.IsConfigured) return;

        if (_connection is not null) builder.UseNpgsql(_connection);
        else if (_connectionString is not null) builder.UseNpgsql(_connectionString);

        builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
    }
}