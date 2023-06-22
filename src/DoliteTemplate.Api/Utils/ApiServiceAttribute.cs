namespace DoliteTemplate.Api.Utils;

[AttributeUsage(AttributeTargets.Class)]
public class ApiServiceAttribute : Attribute
{
    public string? Tag { get; init; }
    public string Rule { get; init; } = ".*";
}