using DoliteTemplate.Api.Resources.Errors;
using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Utils;

public static class World
{
    public static WebApplication App { get; set; } = null!;

    public static IStringLocalizer<SharedDefinitions> SharedDefinitions => Localizer<SharedDefinitions>()!;

    public static IStringLocalizer<SharedErrors> SharedErrors => Localizer<SharedErrors>()!;

    public static IStringLocalizer<T>? Localizer<T>()
    {
        return App.Services.GetService<IStringLocalizer<T>>();
    }
}