using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     路由前缀约定
/// </summary>
public class RoutePrefixConvention(IRouteTemplateProvider route) : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix = new(route);

    public void Apply(ApplicationModel application)
    {
        foreach (var selector in application.Controllers.SelectMany(model => model.Selectors))
        {
            selector.AttributeRouteModel = selector.AttributeRouteModel is not null
                ? AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel)
                : _routePrefix;
        }
    }
}