using DoliteTemplate.Api.Shared.Resources.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Shared.Errors;

/// <summary>
///     Error information
///     <para>Generally used for serialized output</para>
/// </summary>
public class ErrorInfo
{
    /// <summary>
    ///     Error code
    /// </summary>
    public string ErrCode { get; init; } = null!;

    /// <summary>
    ///     Error message
    /// </summary>
    public string ErrMsg { get; init; } = null!;

    /// <summary>
    ///     List of stack trace information
    /// </summary>
    public IEnumerable<string>? Stacktrace { get; init; }

    /// <summary>
    ///     Inner exception
    /// </summary>
    public ErrorInfo? Inner { get; init; }
}

/// <summary>
///     错误信息扩展
/// </summary>
public static class ErrorInfoExtensions
{
    /// <summary>
    ///     Convert from exception to error information
    /// </summary>
    /// <param name="exception">Business or duplicate exception</param>
    /// <param name="app">Current web application</param>
    /// <returns></returns>
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