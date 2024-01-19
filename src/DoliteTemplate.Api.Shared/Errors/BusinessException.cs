namespace DoliteTemplate.Api.Shared.Errors;

/// <summary>
///     Business Exception
/// </summary>
/// <param name="errCode">Error Code</param>
/// <param name="errMsg">Error Message</param>
public class BusinessException(string errCode, string errMsg)
    : Exception($"{errCode}: {errMsg}")
{
    /// <summary>
    ///     Error Code
    /// </summary>
    public string ErrCode { get; } = errCode;

    /// <summary>
    ///     Error Message
    /// </summary>
    public string ErrMsg { get; } = errMsg;
}