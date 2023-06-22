namespace DoliteTemplate.DbMigrator.Seeding;

public static class ConfigurationExtensions
{
    public static IEnumerable<object> GetEntities(this IConfigurationSection section, Type type)
    {
        return section.GetChildren().Select(subSection => subSection.Get(type)).Where(one => one is not null)!;
    }
}