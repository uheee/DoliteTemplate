using DoliteTemplate.Api.Resources.Errors;
using DoliteTemplate.Infrastructure.Utils;
using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Utils.Error;

public class ErrorInfo
{
    public string ErrCode { get; init; } = null!;
    public string ErrMsg { get; init; } = null!;
    public IEnumerable<string>? Stacktrace { get; init; }
    public ErrorInfo? Inner { get; init; }
}

public static class ErrorInfoExtensions
{
    public static ErrorInfo? ToErrorInfo(this Exception? exception, WebApplication app)
    {
        var errorsLocalizer = app.Services.GetService<IStringLocalizer<SharedErrors>>()!;
        var definitionsLocalizer = app.Services.GetService<IStringLocalizer<SharedDefinitions>>()!;
        switch (exception)
        {
            case null:
                return null;
            case BusinessException businessException:
                return new ErrorInfo {ErrCode = businessException.ErrCode, ErrMsg = businessException.ErrMsg};
            case DuplicateException duplicateException:
                const string duplicateError = "{0} exists with duplicate {1}";
                var localizedDuplicateError = errorsLocalizer[duplicateError];
                var entityKey = $"entity: {duplicateException.EntityType.Name}";
                var propertyKeys = duplicateException.PropertyNames
                    .Select(propertyName => $"property: {duplicateException.EntityType.Name}.{propertyName}")
                    .ToArray();
                var localizedEntityKey = definitionsLocalizer[entityKey];
                var localizedPropertyKeys = propertyKeys
                    .Select(propertyKey => definitionsLocalizer[propertyKey])
                    .ToArray();

                return new ErrorInfo
                {
                    ErrCode = string.Format(duplicateError, entityKey, CombineMultipleKeys(propertyKeys)),
                    ErrMsg = string.Format(localizedDuplicateError, localizedEntityKey,
                        CombineMultipleKeys(localizedPropertyKeys))
                };
            default:
                var isDevelopment = app.Environment.IsDevelopment();
                return new ErrorInfo
                {
                    ErrMsg = exception.Message,
                    Stacktrace = exception.StackTrace?.Split(Environment.NewLine).Select(s => s.Trim()),
                    Inner = isDevelopment ? exception.InnerException.ToErrorInfo(app) : null
                };
        }

        string CombineMultipleKeys<T>(IEnumerable<T> keys)
        {
            return string.Join('|', keys);
        }
    }
}