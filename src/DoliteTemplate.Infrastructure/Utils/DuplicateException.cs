namespace DoliteTemplate.Infrastructure.Utils;

public class DuplicateException(Type entityType, params string[] propertyNames) : Exception
{
    public Type EntityType { get; } = entityType;
public string[] PropertyNames { get; } = propertyNames;
}