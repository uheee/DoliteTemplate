namespace DoliteTemplate.Api.Utils.Error;

public class BusinessException(string errCode, string errMsg) : Exception($"{errCode}: {errMsg}")
{
    public string ErrCode { get; } = errCode;
public string ErrMsg { get; } = errMsg;
}