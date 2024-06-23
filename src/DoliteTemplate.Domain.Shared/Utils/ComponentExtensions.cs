namespace DoliteTemplate.Domain.Shared.Utils;

public static class ComponentExtensions
{
    public static string GetComponentName(this Type type, Type baseType)
    {
        return type.Name.EndsWith(baseType.Name) ? type.Name[..^baseType.Name.Length] : type.Name;
    }
}