namespace DoliteTemplate.Api.Utils;

[AttributeUsage(AttributeTargets.Class)]
public class ApiServiceAttribute : Attribute
{
    public ApiServiceAttribute(string? tag = null)
    {
        Tag = tag;
    }

    public string? Tag { get; }
}