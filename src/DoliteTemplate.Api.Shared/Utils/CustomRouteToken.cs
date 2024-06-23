using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace DoliteTemplate.Api.Shared.Utils;

public class CustomRouteToken(string tokenName, Func<ControllerModel, string> valueGenerator)
    : IApplicationModelConvention
{
    private readonly Regex _tokenRegex = new($@"(\[{tokenName}])(?<!\[\1(?=]))");

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var tokenValue = valueGenerator(controller);
            UpdateSelectors(controller.Selectors, tokenValue);
            UpdateSelectors(controller.Actions.SelectMany(a => a.Selectors), tokenValue);
        }
    }

    private void UpdateSelectors(IEnumerable<SelectorModel> selectors, string tokenValue)
    {
        foreach (var selector in selectors.Where(s => s.AttributeRouteModel != null))
        {
            selector.AttributeRouteModel!.Template =
                InsertTokenValue(selector.AttributeRouteModel.Template, tokenValue);
            selector.AttributeRouteModel.Name = InsertTokenValue(selector.AttributeRouteModel.Name, tokenValue);
        }
    }

    private string? InsertTokenValue(string? template, string tokenValue)
    {
        if (template is null)
        {
            return template;
        }

        return _tokenRegex.Replace(template, tokenValue);
    }
}