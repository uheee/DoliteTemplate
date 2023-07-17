namespace DoliteTemplate.Shared;

[AttributeUsage(AttributeTargets.Property)]
public class QueryParameterAttribute : Attribute
{
    public string? Name { get; set; }
    public QueryParameterComparor Comparor { get; set; }
    public object? Default { get; set; }
    public bool IgnoreWhenNull { get; set; }
}

public enum QueryParameterComparor
{
    Eq,
    LtE,
    GtE
}