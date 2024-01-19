using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     MVC配置扩展
/// </summary>
public static class MvcOptionsExtensions
{
    /// <summary>
    ///     使用默认MCV配置
    /// </summary>
    /// <param name="options"></param>
    /// <param name="configuration"></param>
    public static void UseDefaultMvcOptions(this MvcOptions options, IConfiguration configuration)
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new RouteKebabCaseTransformer()));
        var routePrefix = configuration["WebApi:RoutePrefix"];
        if (!string.IsNullOrWhiteSpace(routePrefix))
        {
            options.UseGeneralRoutePrefix(routePrefix);
        }
    }

    /// <summary>
    ///     使用路由前缀
    /// </summary>
    /// <param name="opts">MCV配置</param>
    /// <param name="routeAttribute">路由模板提供器</param>
    public static void UseGeneralRoutePrefix(this MvcOptions opts, IRouteTemplateProvider routeAttribute)
    {
        opts.Conventions.Add(new RoutePrefixConvention(routeAttribute));
    }

    /// <summary>
    ///     使用路由前缀
    /// </summary>
    /// <param name="opts">MCV配置</param>
    /// <param name="prefix">路由前缀</param>
    public static void UseGeneralRoutePrefix(this MvcOptions opts, string prefix)
    {
        opts.UseGeneralRoutePrefix(new RouteAttribute(prefix));
    }
}