namespace DoliteTemplate.Domain.Utils;

[AttributeUsage(AttributeTargets.Property)]
public class QueryParameterAttribute : Attribute
{
    public string? Name { get; set; }
    public QueryParameterComparor Comparor { get; set; }
    public string? Default { get; set; }
}

public enum QueryParameterComparor
{
    Eq,
    LtE,
    GtE
}