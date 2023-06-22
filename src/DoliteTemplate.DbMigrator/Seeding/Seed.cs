using DoliteTemplate.Infrastructure.Utils;

namespace DoliteTemplate.DbMigrator.Seeding;

public class Seed
{
    public DbFilter Filter { get; set; } = null!;
    public string Entity { get; set; } = null!;
    public IConfigurationSection Data { get; set; } = null!;
    public string? Tag { get; set; }
}