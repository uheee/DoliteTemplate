namespace DoliteTemplate.Api.Utils.RestApi;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateAPIAttribute : Attribute
{
    public GenerateAPIAttribute(string? tag = null)
    {
        Tag = tag;
    }

    public string? Tag { get; }
}