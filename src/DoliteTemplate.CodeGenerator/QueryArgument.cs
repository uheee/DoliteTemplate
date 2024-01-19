using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace DoliteTemplate.CodeGenerator;

public class QueryArgument
{
    private static readonly Dictionary<string, PropertyInfo> PropertyMap =
        typeof(QueryArgument).GetProperties().ToDictionary(property => property.Name);

    public QueryArgument(IPropertySymbol propertySymbol,
        ImmutableArray<KeyValuePair<string, TypedConstant>> attributeArguments)
    {
        Property = propertySymbol;
        Name = Extensions.ToCamelCase(propertySymbol.Name);
        Comparor = "{0} == {1}";
        IgnoreWhenNull = true;

        foreach (var attributeArgument in attributeArguments)
        {
            var key = attributeArgument.Key;
            if (!PropertyMap.TryGetValue(key, out var property))
            {
                continue;
            }

            var value = Convert.ChangeType(attributeArgument.Value.Value, property.PropertyType);
            if (key == nameof(Comparor))
            {
                value = ((string)attributeArgument.Value.Value!).ToLower() switch
                {
                    null => value,
                    "eq" => "{0} == {1}",
                    "lt" => "{0} < {1}",
                    "gt" => "{0} > {1}",
                    "lte" => "{0} <= {1}",
                    "gte" => "{0} >= {1}",
                    "contains" => "{0}.Contains({1})",
                    var other => other
                };
            }

            property.SetValue(this, value);
        }

        if (Navigation is not null)
        {
            Name += Navigation?.Replace(".", string.Empty);
        }
    }

    public IPropertySymbol Property { get; set; }
    public string Name { get; set; }
    public string Comparor { get; set; }
    public string? Navigation { get; set; }
    public object? Default { get; set; }
    public bool IgnoreWhenNull { get; set; }
    public string? Description { get; set; }
}