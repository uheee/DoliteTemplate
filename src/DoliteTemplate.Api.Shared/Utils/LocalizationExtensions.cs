using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     本地化扩展
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    ///     使用本地化功能
    /// </summary>
    /// <param name="webApp">Web应用</param>
    /// <param name="configuration">配置项</param>
    public static void UseLocalization(this IApplicationBuilder webApp, IConfiguration configuration)
    {
        webApp.UseRequestLocalization(options =>
        {
            var supportedCultures = configuration.GetSection("Culture:Cultures").Get<string[]>();
            if (supportedCultures is not null)
            {
                options.AddSupportedCultures(supportedCultures);
                options.AddSupportedUICultures(supportedCultures);
            }

            var defaultCulture = configuration["Culture:Default"];
            if (defaultCulture is not null)
            {
                options.SetDefaultCulture(defaultCulture);
            }

            options.ApplyCurrentCultureToResponseHeaders = true;
        });
    }
}