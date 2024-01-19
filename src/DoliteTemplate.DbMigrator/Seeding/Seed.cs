using DoliteTemplate.Infrastructure.Utils;

namespace DoliteTemplate.DbMigrator.Seeding;

/// <summary>
///     数据种子
/// </summary>
public class Seed
{
    /// <summary>
    ///     指定数据库上下文的类型名称
    /// </summary>
    public string DbContext { get; set; } = null!;

    /// <summary>
    ///     数据库过滤器
    /// </summary>
    public DbFilter Filter { get; set; } = null!;

    /// <summary>
    ///     指定实体类型名称
    /// </summary>
    public string Entity { get; set; } = null!;

    /// <summary>
    ///     实体列表
    /// </summary>
    public IConfigurationSection Data { get; set; } = null!;

    /// <summary>
    ///     种子标签
    /// </summary>
    public string? Tag { get; set; }
}