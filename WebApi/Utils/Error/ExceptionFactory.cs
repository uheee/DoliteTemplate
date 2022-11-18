using Microsoft.Extensions.Localization;
using WebApi.Resources.Errors;

namespace WebApi.Utils.Error;

public class ExceptionFactory
{
    public IStringLocalizer<ErrorResource> Localizer { get; init; } = null!;

    public BusinessException Business(int errCode, params object[] args)
    {
        var errTemplate = Localizer[errCode.ToString()];
        var errMsg = string.Format(errTemplate, args);
        return new BusinessException(errCode, errMsg ?? "unknown");
    }
}