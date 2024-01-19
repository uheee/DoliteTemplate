using CaseExtensions;
using Microsoft.AspNetCore.Routing;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     路由短横向命名转换
/// </summary>
public class RouteKebabCaseTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        var route = value?.ToString();
        return string.IsNullOrEmpty(route) ? null : route.ToKebabCase();
    }
}