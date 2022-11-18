using WebApi.Utils.Localization;

namespace WebApi.Utils.Error;

public class ExceptionFactory
{
    public Resource Resource { get; init; } = null!;

    public BusinessException Business(int errCode, params object[] args)
    {
        var errTemplate = Resource["zh_cn"][$"errors:{errCode}"];
        var errMsg = errTemplate is null ? null : string.Format(errTemplate, args);
        return new BusinessException(errCode, errMsg ?? "unknown");
    }
}