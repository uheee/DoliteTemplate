using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DoliteTemplate.Api.Shared.Utils;

/// <summary>
///     JWT登录处理器
///     <remarks>用于响应token过期、处理多地登录等情况</remarks>
/// </summary>
public class JwtBearerExclusiveLoginHandler(
    Lazy<IConnectionMultiplexer> redisProvider,
    IWebHostEnvironment environment,
    IOptionsMonitor<JwtBearerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) :
    JwtBearerHandler(options, logger, encoder)
{
    private bool _expiredFlag;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();
        if (environment.IsDevelopment() || !result.Succeeded)
        {
            return result;
        }

        var currentToken = Request.Headers.Authorization.ToString()[("Bearer".Length + 1)..];
        var userId = result.Ticket.Principal.FindFirstValue(ClaimKeys.UserId);
        var key = $"user:token:{userId}";
        string? cachedToken = await redisProvider.Value.GetDatabase().StringGetAsync(key);
        if (cachedToken is null || string.Equals(currentToken, cachedToken))
        {
            return AuthenticateResult.Success(result.Ticket);
        }

        _expiredFlag = true;
        return AuthenticateResult.Fail("expired_token");
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        await base.HandleChallengeAsync(properties);
        if (_expiredFlag)
        {
            Response.Headers.WWWAuthenticate =
                Response.Headers.WWWAuthenticate.ToString().Replace("invalid_token", "expired_token");
        }
    }
}