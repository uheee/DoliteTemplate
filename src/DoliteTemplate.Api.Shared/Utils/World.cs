using DoliteTemplate.Api.Shared.Resources.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     全局世界对象
///     <remarks>用于记录一些全局对象</remarks>
/// </summary>
public static class World
{
    /// <summary>
    ///     Web应用
    /// </summary>
    public static WebApplication App { get; set; } = null!;

    /// <summary>
    ///     共享定义资源
    /// </summary>
    public static IStringLocalizer<SharedDefinitions> SharedDefinitions => Localizer<SharedDefinitions>()!;

    /// <summary>
    ///     共享错误资源
    /// </summary>
    public static IStringLocalizer<SharedErrors> SharedErrors => Localizer<SharedErrors>()!;

    /// <summary>
    ///     本地化组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IStringLocalizer<T>? Localizer<T>()
    {
        return App.Services.GetService<IStringLocalizer<T>>();
    }
}