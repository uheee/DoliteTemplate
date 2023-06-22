namespace DoliteTemplate.Api.Utils;

public static class LocalizationExtensions
{
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