namespace DoliteTemplate.Infrastructure.Utils;

public class DuplicateException : Exception
{
    public DuplicateException(Type entityType, params string[] propertyNames)
    {
        EntityType = entityType;
        PropertyNames = propertyNames;
    }

    public Type EntityType { get; }
    public string[] PropertyNames { get; }
}