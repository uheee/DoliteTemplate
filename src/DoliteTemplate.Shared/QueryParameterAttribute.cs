namespace DoliteTemplate.Shared;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class QueryParameterAttribute : Attribute
{
    public string? Name { get; set; }
    public string? Comparor { get; set; }
    public object? Default { get; set; }
    public bool IgnoreWhenNull { get; set; }
    public string? Description { get; set; }
}