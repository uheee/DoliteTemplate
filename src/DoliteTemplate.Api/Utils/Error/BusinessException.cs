namespace DoliteTemplate.Api.Utils.Error;

public class BusinessException : Exception
{
    public BusinessException(string errCode, string errMsg) : base($"{errCode}: {errMsg}")
    {
        ErrCode = errCode;
        ErrMsg = errMsg;
    }

    public string ErrCode { get; }
    public string ErrMsg { get; }
}