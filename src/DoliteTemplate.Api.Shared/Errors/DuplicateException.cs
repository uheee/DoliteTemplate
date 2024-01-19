namespace DoliteTemplate.Api.Shared.Errors;

/// <summary>
///     Duplicate entities in database
/// </summary>
/// <param name="entityType">Entity type</param>
/// <param name="propertyNames">Duplicate property names</param>
public class DuplicateException(Type entityType, params string[] propertyNames)
    : Exception($"{entityType}{{{string.Join(", ", propertyNames)}}}")
{
    /// <summary>
    ///     Entity type
    /// </summary>
    public Type EntityType { get; } = entityType;

    /// <summary>
    ///     Duplicate property names
    /// </summary>
    public string[] PropertyNames { get; } = propertyNames;
}