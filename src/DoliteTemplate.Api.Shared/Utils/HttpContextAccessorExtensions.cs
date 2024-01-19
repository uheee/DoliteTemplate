using Microsoft.AspNetCore.Http;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     HTTP上下文访问器扩展
/// </summary>
public static class HttpContextAccessorExtensions
{
    /// <summary>
    ///     获取用户唯一标识
    /// </summary>
    /// <param name="httpContextAccessor">HTTP上下文访问器</param>
    /// <returns></returns>
    public static Guid? GetUserId(this IHttpContextAccessor? httpContextAccessor)
    {
        var claimValue = httpContextAccessor?.HttpContext?.User.FindFirst(ClaimKeys.UserId)?.Value;
        return Guid.TryParse(claimValue, out var id) ? id : null;
    }
}