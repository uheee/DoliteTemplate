namespace WebApi.Utils.Error;

public class ErrorInfo
{
    public int ErrCode { get; init; } = -1;
    public string ErrMsg { get; init; } = null!;
    public IEnumerable<string>? Stacktrace { get; init; }
    public ErrorInfo? Inner { get; init; }
}

public static class ErrorInfoExtensions
{
    public static ErrorInfo? ToErrorInfo(this Exception? exception, bool isDevelopment)
    {
        return exception switch
        {
            null => null,
            BusinessException businessException => new ErrorInfo
            {
                ErrCode = businessException.ErrCode,
                ErrMsg = businessException.ErrMsg
            },
            _ => new ErrorInfo
            {
                ErrMsg = exception.Message,
                Stacktrace = exception.StackTrace?.Split(Environment.NewLine).Select(s => s.Trim()),
                Inner = isDevelopment
                    ? exception.InnerException.ToErrorInfo(isDevelopment)
                    : null
            }
        };
    }
}