namespace DoliteTemplate.Api.Utils;

[AttributeUsage(AttributeTargets.Class)]
public class ApiServiceAttribute : Attribute
{
    public ApiServiceAttribute()
    {
    }

    public ApiServiceAttribute(string tag)
    {
        Tag = tag;
    }

    public string? Tag { get; init; }
}