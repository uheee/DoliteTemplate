namespace DoliteTemplate.DbMigrator.Seeding;

/// <summary>
///     配置项扩展
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    ///     获取配置项中定义的实体
    /// </summary>
    /// <param name="section">实体列表节点</param>
    /// <param name="type">实体类型</param>
    /// <returns></returns>
    public static IEnumerable<object> GetEntities(this IConfigurationSection section, Type type)
    {
        return section.GetChildren().Select(subSection => subSection.Get(type)).Where(one => one is not null)!;
    }
}