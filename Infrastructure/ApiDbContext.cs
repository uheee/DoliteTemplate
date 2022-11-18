using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
        
    }

    public DbSet<Device> Devices { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Device>().HasData(new Device
        {
            Type = "Electric",
            Name = "Computer"
        });
    }
}